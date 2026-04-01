namespace VisualSoft.Surveillance.Radar.Api.Models
{
    public class TransactionFilterRequest
    {
        /// <summary>
        /// Vehicle Registration No 
        /// </summary>
        public string? RadarSourceMac { get; set; }
        /// <summary>
        /// OrganisationId
        /// </summary>
        public Guid? OrganisationId { get; set; }
        /// <summary>
        /// Date From
        /// </summary>
        public DateTime? FromDate { get; set; }
        /// <summary>
        /// Date To
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Source
        /// </summary>
        public string? Source { get; set; }
        /// <summary>
        /// Locations
        /// </summary>
        public string? Location { get; set; }
    }
}
