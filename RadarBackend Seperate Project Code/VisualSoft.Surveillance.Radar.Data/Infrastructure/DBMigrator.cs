using DbUp;
using DbUp.Engine.Transactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Serilog;
using System.Reflection;
using VisualSoft.Surveillance.Radar.Domain;

namespace VisualSoft.Surveillance.Radar.Data.Infrastructure
{
    public class DBMigrator : IDBMigrator
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public DBMigrator(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            using (var scope = _serviceProvider.CreateScope())
            {
                _connectionFactory = scope.ServiceProvider.GetRequiredService<IConnectionFactory>();
            }

            _configuration = configuration;

            NpgsqlConnection Connection = new NpgsqlConnection(_connectionFactory.ConnectionString);
            try
            {
                if (EnsureDB())
                {
                    Connection.Open();
                }

            }
            catch (Exception ex)
            {
                Log.Error($"Can not connect to the database: {Connection.ConnectionString}");
            }
        }

        private bool EnsureDB()
        {
            try
            {
                EnsureDatabase.For.PostgresqlDatabase(_connectionFactory.ConnectionString);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(Constants.Errors.ERROR_ENSURE_DATABASE + ex.Message.ToString());
                return false;
            }
        }
        public void DBMigrate()
        {
            var upgrader = DeployChanges.To
               .PostgresqlDatabase(_connectionFactory.ConnectionString)
               .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
               .WithTransaction()
               .WithVariablesDisabled()
               .Build();
            string error;

            if (!upgrader.TryConnect(out error))
            {
                Log.Error($"Database Error: could not connect to Database: ${error}");
                return;
            }

            var result = upgrader.PerformUpgrade();
            if (!result.Successful)
            {
                Log.Error($"Database upgrade failed with error: {result.Error}");
            }
        }
    }
}
