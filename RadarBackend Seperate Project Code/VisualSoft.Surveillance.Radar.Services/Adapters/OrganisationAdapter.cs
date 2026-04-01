using Microsoft.Extensions.Caching.Memory;
using Serilog;
using VisualSoft.Surveillance.Radar.Domain;
using VisualSoft.Surveillance.Radar.Domain.Configurations;
using VisualSoft.Surveillance.Radar.Domain.Models;
using VisualSoft.Surveillance.Radar.Services.Infrastructure;

namespace VisualSoft.Surveillance.Radar.Services.Adapters
{
    public class OrganisationAdapter : AdapterTemplate, IOrganisationAdapter
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger _logger;

        public OrganisationAdapter(IServiceConfiguration serviceConfiguration, IAuthTokenExtractor authTokenExtractor,
                           IMemoryCache memoryCache, ILogger logger, IHttpClientFactory httpClientFactory)
                                    : base(serviceConfiguration, authTokenExtractor, httpClientFactory)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }


        public async Task<OrganisationDto?> GetOrganisationById(Guid organisationId)
        {
            var organisationsWithPermissionsDto = await GetOrganisations();
            var organisation = organisationsWithPermissionsDto.Organisations.Where(x => x.Id == organisationId).FirstOrDefault();

            var organisationAccessRights = organisationsWithPermissionsDto.AccessRights.Where(x => x.OrganisationId == organisationId).FirstOrDefault();
            organisation.AccessRights = organisationAccessRights?.AccessRights;

            return organisation;


        }
        public async Task<OrganisationsWithPermissionsDto?> GetOrganisations()
        {
            OrganisationsWithPermissionsDto? organisationsWithPermissionsDto = null; 
            try
            {
                var devicesCacheKey = $"{DateTime.Now.ToString("ddMMyyyy")}_organisations";

                if (_memoryCache.TryGetValue(devicesCacheKey, out organisationsWithPermissionsDto))
                {
                    return organisationsWithPermissionsDto;
                }

                HttpClient client = AuthenticatedClient;
                // List data response.
                HttpResponseMessage response = await client.GetAsync(_serviceConfiguration.GetAllOrganisationsUrl());
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStreamAsync();
                    if (responseContent != null)
                    {
                        organisationsWithPermissionsDto = await ExtensionMethods.DeserializeAsync<OrganisationsWithPermissionsDto>(responseContent);

                        if (organisationsWithPermissionsDto != null && organisationsWithPermissionsDto.Organisations.Count() > 0)
                        {
                            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24));
                            _memoryCache.Set(devicesCacheKey, organisationsWithPermissionsDto, cacheEntryOptions);
                            return organisationsWithPermissionsDto;
                        }
                    }
                }
            }
            catch(Exception exp)
            {
                _logger.Error(exp, "Error Occurred");
            }
            return organisationsWithPermissionsDto;
        }
    }
}
