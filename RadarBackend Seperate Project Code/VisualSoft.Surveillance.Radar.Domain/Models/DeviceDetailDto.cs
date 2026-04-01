using System.Text.Json.Serialization;

namespace VisualSoft.Surveillance.Radar.Domain.Models
{
    public class DeviceDetailDto : BaseDto
    {
        public Guid Id { get; set; }
        public Guid? OrganisationId { get; set; }
        public string OrganisationName { get; set; }

        public string Provider { get; set; }
        public string SerialNumber { get; set; }
        public string Category { get; set; }
        public string DeviceName { get; set; }
        public string Model { get; set; }
        public string MacAddress { get; set; }
        public IEnumerable<DeviceSettingDto>? Settings { get; set; }

       
    }
}
