namespace VisualSoft.Surveillance.Radar.Domain.Models
{
    public class TransactionFilterRequestDto
    {
        public Guid? OrganisationId { get; set; }
        public string? RadarSourceMac { get; set; }
        public string? DeviceName { get; set; }
        public string? Location { get; set; }
        public string? Source { get; set; }
        public bool? Presence { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
