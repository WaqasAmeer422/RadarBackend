using System.Text.Json;
using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Domain
{
    public static class ExtensionMethods
    {
        public static async Task<T?> DeserializeAsync<T>(ReadOnlyMemory<byte> message)
        {
            using (var stream = new MemoryStream(message.ToArray()))
            {
                T? result = await JsonSerializer.DeserializeAsync<T>(stream);
                return result;
            }
        }

        public static async Task<T?> DeserializeAsync<T>(Stream message)
        {
            T? result = await JsonSerializer.DeserializeAsync<T>(message);
            return result;
        }

        public static byte[] SerializeToUtf8Bytes<T>(IEnumerable<T> model)
        {
            return JsonSerializer.SerializeToUtf8Bytes(model);
        }

        public static byte[] SerializeToUtf8Bytes<T>(T model)
        {
            return JsonSerializer.SerializeToUtf8Bytes(model);
        }

        public static T? Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T?);
            }
            return JsonSerializer.Deserialize<T>(json);
        }

        public static string Serialize<T1>(T1 model)
        {
            if (model == null)
            {
                return string.Empty;
            }
            return JsonSerializer.Serialize(model);
        }


       

        public static string TransformDeviceSetting(string? key, string? value)
        {
            return $"{key} : {value}";
        }
    }
}
