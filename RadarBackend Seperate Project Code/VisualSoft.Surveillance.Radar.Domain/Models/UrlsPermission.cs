using System.Text.Json.Serialization;

namespace VisualSoft.Surveillance.Radar.Domain.Models
{
    public class UrlsPermission
    {
        public string Key { get; set; }

        [JsonIgnore]
        public string Route { get; set; }

        public Uri Url { get; set; }

        public string Permission { get; set; }
    }
}
