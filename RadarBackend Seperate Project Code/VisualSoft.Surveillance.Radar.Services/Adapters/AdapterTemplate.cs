using System.Net.Http.Headers;
using VisualSoft.Surveillance.Radar.Domain.Configurations;
using VisualSoft.Surveillance.Radar.Services.Infrastructure;

namespace VisualSoft.Surveillance.Radar.Services.Adapters
{
    public abstract class AdapterTemplate
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        private DateTime tokenCreatedAt;
        protected readonly IAuthTokenExtractor _authTokenExtractor;
        protected readonly IServiceConfiguration _serviceConfiguration;
        private HttpClient httpClient;

        protected string Token { get; set; } = string.Empty;

        // Use a lazy-initialized HttpClient to avoid creating it multiple times
        private readonly Lazy<Task<HttpClient>> _authenticatedClient;
        protected HttpClient AuthenticatedClient => _authenticatedClient.Value.Result;


        protected AdapterTemplate(IServiceConfiguration serviceConfiguration, IAuthTokenExtractor authTokenExtractor, IHttpClientFactory httpClientFactory)
        {
            _serviceConfiguration = serviceConfiguration;
            _authTokenExtractor = authTokenExtractor;
            _httpClientFactory = httpClientFactory;

            // Initialize the lazy factory to create the HttpClient when first accessed
            _authenticatedClient = new Lazy<Task<HttpClient>>(CreateAuthenticatedClient);
        }


        private async Task<HttpClient> CreateAuthenticatedClient()
        {
            // Get the pre-configured client from the factory
            var client = _httpClientFactory.CreateClient("AuthenticatedClient");

            // Get the token from the extractor
            if (string.IsNullOrWhiteSpace(Token))
            {
                Token = await _authTokenExtractor.Extract();
                tokenCreatedAt = DateTime.Now;
            }

            if (!string.IsNullOrWhiteSpace(Token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthTokenExtractor.BearerSchemePrefix, Token);
            }

            return client;
        }
    }
}
