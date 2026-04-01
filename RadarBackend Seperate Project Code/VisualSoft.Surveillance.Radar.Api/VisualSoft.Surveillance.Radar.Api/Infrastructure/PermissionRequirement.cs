using Microsoft.AspNetCore.Authorization;

namespace VisualSoft.Surveillance.Radar.Api.Infrastructure
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; private set; }
        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
