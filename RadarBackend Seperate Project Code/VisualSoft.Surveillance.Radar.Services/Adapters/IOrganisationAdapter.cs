using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Services.Adapters
{
    public interface IOrganisationAdapter
    {
        Task<OrganisationsWithPermissionsDto?> GetOrganisations();
        Task<OrganisationDto?> GetOrganisationById(Guid organisationId);
    }
}
