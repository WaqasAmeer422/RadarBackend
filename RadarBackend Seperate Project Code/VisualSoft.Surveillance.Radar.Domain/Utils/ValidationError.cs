using System.Text.Json.Serialization;

namespace VisualSoft.Surveillance.Radar.Domain.Utils
{
    public class ValidationError
    {
        public ValidationError(string field, string message)
        {
            Field = field;
            Message = message;
        }

        [JsonPropertyName("field")]
        public string Field { get; private set; }
        [JsonPropertyName("message")]
        public string Message { get; private set; }
    }
}
