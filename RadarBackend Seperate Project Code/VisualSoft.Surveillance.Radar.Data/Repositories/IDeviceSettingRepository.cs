using VisualSoft.Surveillance.Radar.Data.Models;
using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Data.Repositories
{
    public interface IDeviceSettingRepository
    {
        Task<IEnumerable<DeviceSettingDto>?> CreateDeviceSettings(Guid deviceId, List<DeviceSettingModel> settings);
        Task<IEnumerable<DeviceSettingDto>?> GetDeviceSettingsByDeviceId(Guid deviceid);
        Task<IEnumerable<DeviceSettingDto>?> GetAllDeviceSettings();
        Task<IEnumerable<DeviceSettingDto>?> GetDeviceSettingsByCriteria(DeviceSettingDto? model);
        Task DeleteDeviceSettings(IList<Guid> deviceIds);
        Task UpdateDeviceSettings(DeviceSettingDto setting);
    }
}
