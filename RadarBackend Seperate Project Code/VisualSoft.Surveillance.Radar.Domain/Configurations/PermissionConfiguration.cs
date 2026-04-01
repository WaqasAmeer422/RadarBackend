using System.Text.Json.Serialization;
using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Domain.Configurations
{
    public class PermissionConfiguration
    {
        public string ApiName { get; set; }

        [JsonIgnore]
        public string ApiUrl { get; set; }

        public IEnumerable<UrlsPermission> UrlsPermissions { get; set; }
    }
}
