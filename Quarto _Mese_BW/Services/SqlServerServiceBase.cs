using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Quarto__Mese_BW.Services
{
    public class SqlServerServiceBase : ServiceBase
    {
        private readonly SqlConnection _connection;

        public SqlServerServiceBase(IConfiguration config)
        {
            _connection = new SqlConnection(config.GetConnectionString("ECommerce"));
        }

        protected override DbCommand GetCommand(string commandText)
        {
            return new SqlCommand(commandText, _connection);
        }

        protected override DbConnection GetConnection()
        {
            return _connection;
        }
    }
}
