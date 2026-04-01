namespace VisualSoft.Surveillance.Radar.Data.Models
{
    public class DeviceSettingModel : BaseModel
    {
        public Guid Id { get; set; }
        public Guid? DeviceId { get; set; }
        public string SettingName { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
    }
}
