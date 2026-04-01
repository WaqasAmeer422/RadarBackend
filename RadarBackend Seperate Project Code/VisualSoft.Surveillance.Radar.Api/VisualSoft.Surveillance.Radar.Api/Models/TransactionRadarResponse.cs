using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Api.Models
{
    public class TransactionRadarResponse : BaseResponse
    {
        public Guid Id { get; set; }
        //public Guid? DeviceId { get; set; }
        public Guid? OrganisationId { get; set; } //OrgID
        public string OrganisationName { get; set; }
        public Guid? DeviceId { get; set; }
        public string TransactionEvent { get; set; } //Source
        public string Location { get; set; }
        public string Level1Location { get; set; }
        public string Level2Location { get; set; }

        public string RadarSourceMac { get; set; }
        //public string AgrinodeMac { get; set; }
        public bool Presence { get; set; }
        public DateTime DeviceTimestamp { get; set; }
       // public DateTime CreatedDate { get; set; }
        
    }
}