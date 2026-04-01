using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using VisualSoft.Surveillance.Radar.Domain.Configurations;
using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Services.Infrastructure
{
    public class ServiceUserAuthTokenExtractor : IAuthTokenExtractor
    {
        private readonly IServiceConfiguration _serviceConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;

        public ServiceUserAuthTokenExtractor(IServiceConfiguration serviceConfiguration, IHttpClientFactory httpClientFactory)
        {
            _serviceConfiguration = serviceConfiguration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string?> Extract()
        {
            var client = _httpClientFactory.CreateClient("TokenClient");

            var header = _serviceConfiguration.GetServiceUserHeaderConfiguration();
            client.DefaultRequestHeaders.Add(header.Key, header.Value);
            // The token login URL is dynamic, so we set it here
            var loginUrl = _serviceConfiguration.GetLoginUrl();
            var requestBody = _serviceConfiguration.GetServiceLoginRequest();

            HttpResponseMessage response = await client.PostAsJsonAsync(loginUrl, requestBody);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<LoginResponse>(responseContent).Token;
            }
            return null;
        }
    }
}
