using VisualSoft.Surveillance.Radar.Data.Models;
using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Data.Repositories
{
    public interface ILookupRepository
    {
        Task<IEnumerable<LookupDto>?> GetAllLookups();
        Task<IEnumerable<LookupDto>?> GetLookupByCriteria(LookupDto? model);
    }
}
