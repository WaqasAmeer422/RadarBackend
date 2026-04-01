using System.Text.Json.Serialization;

namespace VisualSoft.Surveillance.Radar.Domain.Models
{
    public class LoginRequest
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
