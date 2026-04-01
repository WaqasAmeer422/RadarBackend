using System.Data;
using VisualSoft.Surveillance.Radar.Data.Repositories;
using VisualSoft.Surveillance.Radar.Domain;
using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Data.Infrastructure
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;

        private readonly IDbConnection connection;
        private IDbTransaction transaction;
        private IUserIdentificationModel? logedinUser = null;

        public UnitOfWork(IConnectionFactory connectionFactory)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            _connectionFactory = connectionFactory;
            connection = _connectionFactory.Connection;
            connection.Open();
        }

        public IUserIdentificationModel? LogedinUser
        {
            get
            {
                return logedinUser;
            }
            set
            {
                logedinUser = value;
            }
        }

        public IDbTransaction? Transaction => transaction;
        public IDbConnection Connection => connection;
        public IDeviceRepository DeviceRepository => new DeviceRepository(Connection, LogedinUser);
        public ILookupRepository LookupRepository => new LookupRepository(Connection, LogedinUser);
        
        public IDeviceSettingRepository DeviceSettingRepository => new DeviceSettingRepository(Connection, LogedinUser);
        //public ITransactionLogRepository TransactionLogRepository => new TransactionLogRepository(Connection, LogedinUser);
        // THIS is for RADAR
        public IRadarTransactionRepository RadarTransactionRepository => new RadarTransactionRepository(Connection, LogedinUser);
        public void BeginTransaction()
        {
            if (Connection.State.Equals(ConnectionState.Closed))
            {
                Connection.Open();
            }
            transaction = Connection.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                connection.Close();
                // Dispose();
            }
        }

        public void Rollback()
        {
            transaction.Rollback();
            // Dispose();
        }

        public void Dispose()
        {
            transaction?.Dispose();
            connection?.Close();
            connection?.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
