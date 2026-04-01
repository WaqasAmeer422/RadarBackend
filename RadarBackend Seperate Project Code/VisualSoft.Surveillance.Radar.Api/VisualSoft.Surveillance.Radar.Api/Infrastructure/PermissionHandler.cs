using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace VisualSoft.Surveillance.Radar.Api.Infrastructure
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement permissionRequirement)
        {
            if (context.IsAuthenticated() && HasValue(context.User, permissionRequirement))
                context.Succeed(permissionRequirement);
            return Task.CompletedTask;
        }

        private bool HasValue(ClaimsPrincipal user, PermissionRequirement permissionRequirement)
        {
            if (permissionRequirement != null && user != null && user.Claims != null)
                return user.Claims.Any(claim => !string.IsNullOrWhiteSpace(claim.Type) && claim.Type.Equals(permissionRequirement.Permission));
            return false;
        }
    }
}
