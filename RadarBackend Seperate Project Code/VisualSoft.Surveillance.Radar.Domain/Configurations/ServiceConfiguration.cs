using Microsoft.Extensions.Configuration;
using System.Text;
using VisualSoft.Surveillance.Radar.Domain.Models;
using VisualSoft.Surveillance.Radar.Domain.Models;


namespace VisualSoft.Surveillance.Radar.Domain.Configurations
{
    public interface IServiceConfiguration
    {
        string? PostgresConnectionString { get; }
        
        string GetLoginUrl();
        string GetAllOrganisationsUrl();
        CustomHeaderConfigurations? GetServiceUserHeaderConfiguration();
        LoginRequest GetServiceLoginRequest();

    }
    public class ServiceConfiguration : IServiceConfiguration
    {
        private readonly IConfiguration _configuration;
        private List<ApiLinksConfigurations>? apiLinksConfigurations { get; set; }
        private List<CustomHeaderConfigurations>? customHeaderConfigurations { get; set; }
        private List<HostedServicesConfigurations>? hostedServicesConfigurations { get; set; }

        private const string MANAGEMENT_API = "VisualSoft.Surveillance.Management.Api";
        

        public ServiceConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;

            customHeaderConfigurations = configuration.GetSection("Custom-Headers").Get<List<CustomHeaderConfigurations>>();
            hostedServicesConfigurations = configuration.GetSection("HostedServices").Get<List<HostedServicesConfigurations>>();

            apiLinksConfigurations = configuration.GetSection("ApiLinks").Get<List<ApiLinksConfigurations>>();
            var urlConfigs = configuration.GetSection("ApiLinkUrl").AsEnumerable().ToList();
            foreach (var apiLinksConfiguration in apiLinksConfigurations)
            {
                apiLinksConfiguration.BaseUrl = urlConfigs.FirstOrDefault(x => x.Key == $"ApiLinkUrl:{apiLinksConfiguration.ApiName}").Value ?? string.Empty;
            }

        }

        public LoginRequest GetServiceLoginRequest()
        {
            var loginRequest = _configuration.GetSection("Service-Login").Get<LoginRequest>();

            var data = Convert.FromBase64String(loginRequest.Password);
            var decodedString = Encoding.UTF8.GetString(data);
            loginRequest.Password = decodedString;

            return loginRequest;
        }

        public CustomHeaderConfigurations? GetServiceUserHeaderConfiguration()
        {
            return customHeaderConfigurations?.Find(h => h.Key == "vs_service_auth_token");
        }
        public string GetLoginUrl()
        {
            return Compose(MANAGEMENT_API, "Login");
        }

        public string GetAllOrganisationsUrl()
        {
            return Compose(MANAGEMENT_API, "AllOrganisations");
        }


        public string? PostgresConnectionString
        {
            get
            {
                return _configuration.GetValue<string?>("Connections:PostgresConnectionString");
            }
        }
        private string Compose(string apiName, string key)
        {
            var api = apiLinksConfigurations.FirstOrDefault(x => x.ApiName.Equals(apiName));
            var link = api.Links.FirstOrDefault(x => x.Key.Equals(key));

            if (link == null)
            {
                throw new ArgumentException($"Api link does not exist for key {key}");
            }

            return $"{api.BaseUrl}{link.Path}";
        }
    }
}
