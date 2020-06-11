using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApplication1
{
    public abstract class RepositoryBase
    {
        private static readonly MethodInfo _changeTypeMethod = typeof(Convert).GetMethod(nameof(Convert.ChangeType), new[] { typeof(object), typeof(Type) });
        private static readonly MethodInfo _getValueMethod = typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetValue), new[] { typeof(int) });

        private readonly ILogger _logger;
        private readonly string _masterConnectionString;

        protected RepositoryBase(IConfiguration configuration, ILogger logger)
        {
            (this._masterConnectionString, this._logger) = (configuration.GetValue<string>("ConnectionString"), logger);
        }

        public void BulkInsert<T>(IEnumerable<T> data, string name)
            where T : new()
        {
            _logger.LogInformation("Bulk inserting to database 'localdb'...");
            using SqlConnection connection = this.GetConnection();
            connection.Open();
            connection.ChangeDatabase("localdb");
            using SqlCommand command = new SqlCommand($"select {DbDataReaderAdapter<T>.Initializer} into {name} where 1=0;", connection);
            command.ExecuteNonQuery();
            using SqlBulkCopy bulk = new SqlBulkCopy(connection)
            {
                DestinationTableName = name,
                BatchSize = 10_000,
                EnableStreaming = true
            };
            using DbDataReader reader = new DbDataReaderAdapter<T>(data);
            bulk.WriteToServer(reader);
            _logger.LogInformation("Done!");
        }

        public IEnumerable<T> GetData<T>(string name)
            where T : new()
        {
            _logger.LogInformation("Reading from database 'localdb'...");
            using SqlConnection connection = this.GetConnection();
            connection.Open();
            connection.ChangeDatabase("localdb");
            using SqlCommand command = new SqlCommand($"select * from {name}", connection);
            command.ExecuteNonQuery();
            using SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                yield return ObjectCreator<T>.CreateItem(dataReader);
            }
            _logger.LogInformation("Done!");
        }

        public void SetupLocalDb()
        {
            _logger.LogInformation("Creating database 'localdb'...");
            using SqlConnection connection = this.GetConnection();
            connection.Open();
            using SqlCommand command = new SqlCommand($@"create database localdb on (name='localdb', fileName='{Path.GetTempFileName()}.mdf'); alter database localdb collate latin1_general_bin2", connection);
            command.ExecuteNonQuery();
            _logger.LogInformation("Done!");
        }

        public void TeardownLocalDb()
        {
            _logger.LogInformation("Destroying database 'localdb'...");
            using SqlConnection connection = this.GetConnection();
            connection.Open();
            using SqlCommand cmd = new SqlCommand(@"
select top 1 physical_name from sys.master_files where db_id('localdb')=database_id;
if db_id('localdb') is not null
begin
    alter database localdb set single_user with rollback immediate
    exec sp_detach_db 'localdb'
end", connection);
            if (!(cmd.ExecuteScalar() is string text))
            {
                return;
            }
            string path = text[0..^4];

            static void TryDelete(string path)
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                }
            }

            TryDelete(path);
            TryDelete($"{path}.mdf");
            TryDelete($"{path}_log.ldf");
            _logger.LogInformation("Done!");
        }

        protected abstract SqlConnection GetConnection();

        private static (Func<T, object[]>, string) CreateDataReaderInitializers<T>()
        {
            StringBuilder initializer = new StringBuilder();
            ParameterExpression item = Expression.Parameter(typeof(T), nameof(item));
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<Expression> expressions = new List<Expression>();
            foreach (PropertyInfo property in properties.Where(prop => prop.CanRead && prop.CanWrite))
            {
                if (expressions.Count != 0)
                {
                    initializer.Append(", ");
                }
                expressions.Add(
                    Expression.Convert(
                        Expression.Property(
                            item,
                            property
                        ),
                        typeof(object)
                    )
                );
                initializer.Append(property.Name).Append('=').Append(GetFieldInitializer(property.PropertyType));
            }
            Expression<Func<T, object[]>> lambda = Expression.Lambda<Func<T, object[]>>(
                Expression.NewArrayInit(
                    typeof(object),
                    expressions
                ),
                item
            );
            return (lambda.Compile(), initializer.ToString());
        }

        private static Func<DbDataReader, T> CreateObjectReaderProjection<T>()
            where T : new()
        {
            ParameterExpression reader = Expression.Parameter(typeof(DbDataReader), nameof(reader));
            Expression<Func<DbDataReader, T>> lambda = Expression.Lambda<Func<DbDataReader, T>>(
                Expression.MemberInit(
                    Expression.New(
                        typeof(T)
                    ),
                    typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(prop => prop.CanRead && prop.CanWrite).Select((property, index) => Expression.Bind(
                        property,
                        Expression.Convert(
                            Expression.Call(
                                null,
                                _changeTypeMethod,
                                Expression.Call(
                                    reader,
                                    _getValueMethod,
                                    Expression.Constant(
                                        index
                                    )
                                ),
                                Expression.Constant(
                                    property.PropertyType
                                )
                            ),
                            property.PropertyType
                        )
                    ))
                ),
                reader
            );
            return lambda.Compile();
        }

        private static string GetFieldInitializer(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.Int16:
                case TypeCode.Int32:
                    return "0";
                case TypeCode.Single:
                case TypeCode.Decimal:
                case TypeCode.Double:
                    return "0.0";
            }
            return "space(8000)";
        }

        private static class ObjectCreator<T>
            where T : new()
        {
            private static readonly Func<DbDataReader, T> _projection = CreateObjectReaderProjection<T>();

            public static T CreateItem(DbDataReader dataReader) => _projection(dataReader);
        }

        private sealed class DbDataReaderAdapter<T> : DbDataReader
        {
            private static readonly Func<T, object[]> _arrayProjection;

            private readonly IEnumerator<object[]> _enumerator;
            private StrongBox<bool> _firstRead = new StrongBox<bool>();

            static DbDataReaderAdapter() => (_arrayProjection, Initializer) = CreateDataReaderInitializers<T>();

            public DbDataReaderAdapter(IEnumerable<T> data)
            {
                _enumerator = data.Select(_arrayProjection).GetEnumerator();
            }

            public static string Initializer { get; }

            public override int Depth => throw new NotImplementedException();

            public override int FieldCount => (_firstRead.Value = _enumerator.MoveNext()) ? _enumerator.Current.Length : 0;

            public override bool HasRows => throw new NotImplementedException();

            public override bool IsClosed => throw new NotImplementedException();

            public override int RecordsAffected => throw new NotImplementedException();

            public override object this[int ordinal] => throw new NotImplementedException();

            public override object this[string name] => throw new NotImplementedException();

            public override bool GetBoolean(int ordinal) => throw new NotImplementedException();

            public override byte GetByte(int ordinal) => throw new NotImplementedException();

            public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) => throw new NotImplementedException();

            public override char GetChar(int ordinal) => throw new NotImplementedException();

            public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) => throw new NotImplementedException();

            public override string GetDataTypeName(int ordinal) => throw new NotImplementedException();

            public override DateTime GetDateTime(int ordinal) => throw new NotImplementedException();

            public override decimal GetDecimal(int ordinal) => throw new NotImplementedException();

            public override double GetDouble(int ordinal) => throw new NotImplementedException();

            public override IEnumerator GetEnumerator() => throw new NotImplementedException();

            public override Type GetFieldType(int ordinal) => throw new NotImplementedException();

            public override float GetFloat(int ordinal) => throw new NotImplementedException();

            public override Guid GetGuid(int ordinal) => throw new NotImplementedException();

            public override short GetInt16(int ordinal) => throw new NotImplementedException();

            public override int GetInt32(int ordinal) => throw new NotImplementedException();

            public override long GetInt64(int ordinal) => throw new NotImplementedException();

            public override string GetName(int ordinal) => throw new NotImplementedException();

            public override int GetOrdinal(string name) => throw new NotImplementedException();

            public override string GetString(int ordinal) => throw new NotImplementedException();

            public override object GetValue(int ordinal) => _enumerator.Current[ordinal];

            public override int GetValues(object[] values) => throw new NotImplementedException();

            public override bool IsDBNull(int ordinal) => _enumerator.Current[ordinal] is null;

            public override bool NextResult() => throw new NotImplementedException();

            public override bool Read() => Interlocked.Exchange(ref _firstRead, null) is StrongBox<bool> box ? box.Value : _enumerator.MoveNext();

            protected override void Dispose(bool disposing) => _enumerator.Dispose();
        }
    }
}
