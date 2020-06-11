using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApplication1
{
    public sealed class SingletonRepository : RepositoryBase
    {
        private readonly Func<SqlConnection> _connectionFactory;

        public SingletonRepository(IConfiguration configuration, Func<SqlConnection> connectionFactory, ILogger<SingletonRepository> logger)
            : base(configuration, logger)
        {
            this._connectionFactory = connectionFactory;
        }

        protected override SqlConnection GetConnection() => this._connectionFactory();
    }
}
