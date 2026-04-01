using DbUp.Engine.Transactions;
using Npgsql;
using VisualSoft.Surveillance.Radar.Domain.Configurations;

namespace VisualSoft.Surveillance.Radar.Data.Infrastructure
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly IServiceConfiguration _serviceConfiguration;

        public ConnectionFactory(IServiceConfiguration serviceConfiguration)
        {
            _serviceConfiguration = serviceConfiguration;
        }

        public string ConnectionString
        {
            get
            {
                if (_serviceConfiguration.PostgresConnectionString == null)
                {
                    throw new ArgumentNullException(nameof(_serviceConfiguration));
                }
                else
                {
                    return _serviceConfiguration.PostgresConnectionString;
                }
            }
        }

        public NpgsqlConnection Connection
        {
            get
            {
                return CreateNewConnection();
            }
        }

        private NpgsqlConnection CreateNewConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }
    }
}
