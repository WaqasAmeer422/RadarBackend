using System.Text.Json.Serialization;

namespace VisualSoft.Surveillance.Radar.Domain.Models
{
    public class RadarEventModel
    {
        [JsonPropertyName("RadarSource_mac")]
        public string RadarSourceMac { get; set; }

        [JsonPropertyName("Agri_mac")]
        public string AgrinodeMac { get; set; }

        [JsonPropertyName("presence")]
        public bool Presence { get; set; }

        [JsonPropertyName("pres_type")]
        public string PresenceType { get; set; } // "stationary" or "moving"

        [JsonPropertyName("device_timestamp")]
        public string DeviceTimestamp { get; set; } 
    }
}