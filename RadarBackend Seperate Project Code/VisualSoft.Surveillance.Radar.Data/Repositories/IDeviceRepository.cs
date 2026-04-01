using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Data.Repositories
{
    public interface IDeviceRepository
    {
        Task<DeviceDetailDto?> Create(DeviceDetailDto model);
        Task<DeviceDetailDto?> GetById(Guid id);
        Task<IEnumerable<DeviceDetailDto>?> GetAll(Guid organisation_id);
        Task<IEnumerable<DeviceDetailDto>?> GetAll();
        Task<DeviceDetailDto?> Update(DeviceDetailDto model);
        Task<IEnumerable<DeviceDetailDto>?> GetByCriteria(DeviceDetailDto? model);
        Task<DeviceDetailDto?> GetByMacAddress(string macaddress);
        Task<(IEnumerable<DeviceDetailDto>?, int totalCount)> GetAllPagedItems(PaginationParameters paginationParams);
        Task<(IEnumerable<DeviceDetailDto>?, int totalCount)> GetAllPagedItems(Guid organisation_id, PaginationParameters paginationParams);
        Task<(IEnumerable<DeviceDetailDto>?, int totalCount)> GetByCriteriaPagedItems(DeviceDetailDto model, PaginationParameters paginationParams);
    }
}
