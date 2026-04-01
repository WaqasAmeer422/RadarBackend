using System.Security.Claims;

namespace VisualSoft.Surveillance.Radar.Services.Infrastructure
{
    public class AuthTokenExtractor : IAuthTokenExtractor
    {
        public const string BearerSchemePrefix = "Bearer";

        public async Task<string?> Extract()
        {
            string authToken = string.Empty;

            if (Thread.CurrentPrincipal != null)
            {
                var ci = Thread.CurrentPrincipal.Identity as ClaimsIdentity;

                if (ci != null && ci.BootstrapContext != null)
                {
                    authToken = ci.BootstrapContext.ToString() ?? string.Empty;
                }
            }

            return authToken;
        }
    }
}
