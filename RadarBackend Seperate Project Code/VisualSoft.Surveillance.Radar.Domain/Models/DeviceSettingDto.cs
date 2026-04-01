namespace VisualSoft.Surveillance.Radar.Domain.Models
{
    public class DeviceSettingDto
    {

        public Guid Id { get; set; }

        public Guid? DeviceId { get; set; }

        public string SettingName { get; set; }

        public string SettingKey { get; set; }

        public string SettingValue { get; set; }
        public string SettingKeyDescription { get; set; }

        public string SettingValueDescription { get; set; }

        public string TransformedSettingValue
        {
            get
            {
                return ExtensionMethods.TransformDeviceSetting(SettingKeyDescription, 
                    string.IsNullOrWhiteSpace(SettingValueDescription) ? SettingValue : SettingValueDescription);
            }
        }

    }
}
