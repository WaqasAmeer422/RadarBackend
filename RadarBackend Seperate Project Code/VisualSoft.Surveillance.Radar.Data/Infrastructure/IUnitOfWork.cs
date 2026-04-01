using System.Data;
using VisualSoft.Surveillance.Radar.Data.Repositories;
using VisualSoft.Surveillance.Radar.Domain;
using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Data.Infrastructure
{
    public interface IUnitOfWork
    {
        IUserIdentificationModel LogedinUser { get; set; }
        IDbTransaction Transaction { get; }

        IDeviceRepository DeviceRepository { get; }
        IDeviceSettingRepository DeviceSettingRepository { get; }
        IDbConnection Connection { get; }
        // radar repositry
        IRadarTransactionRepository RadarTransactionRepository { get; }
        ILookupRepository LookupRepository { get; }
        void BeginTransaction();
        void Commit();
        void Rollback();

    }
}
