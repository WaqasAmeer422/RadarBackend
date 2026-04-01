using Microsoft.AspNetCore.Authorization;

namespace VisualSoft.Surveillance.Radar.Api.Infrastructure
{
    public static class AuthorizationHandlerContextExtension
    {
        public static bool IsAuthenticated(this AuthorizationHandlerContext context)
        {
            return context != null && context.User != null && context.User.Identities.Any(identity => identity.IsAuthenticated);
        }
    }
}
