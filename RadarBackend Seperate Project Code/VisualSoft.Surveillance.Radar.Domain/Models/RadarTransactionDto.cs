namespace VisualSoft.Surveillance.Radar.Domain.Models
{
    public class RadarTransactionDto : BaseDto
    {
        public Guid Id { get; set; } //primary key
        public Guid? OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public Guid? DeviceId { get; set; }
        public string TransactionEvent { get; set; }       
        public string Location { get; set; }
        public string Level1Location { get; set; }
        public string Level2Location { get; set; }
        public string RadarSourceMac { get; set; } = string.Empty;
        public string AgrinodeMac { get; set; } = string.Empty;
        public bool Presence { get; set; }
        public string PresenceType { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime? DeviceTimestamp { get; set; }
        
    }
}