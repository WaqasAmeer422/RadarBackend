using System.Text.Json.Serialization;

namespace VisualSoft.Surveillance.Radar.Domain.Models
{
    public class LoginResponse
    {
        /// <summary>
        /// Jwt Token
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }
}
