using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApplication1
{
    public sealed class TransientRepository : RepositoryBase
    {
        private readonly SqlConnection _connection;

        public TransientRepository(IConfiguration configuration, SqlConnection connection, ILogger<TransientRepository> logger)
           : base(configuration, logger)
        {
            this._connection = connection;
        }

        protected override SqlConnection GetConnection() => this._connection;
    }
}
