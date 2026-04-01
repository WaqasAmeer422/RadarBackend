namespace VisualSoft.Surveillance.Radar.Api.Models
{
    public class BaseResponse
    {
        /// <summary>
        /// Status showing if record is active or not
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// DateTime record been created
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// DateTime record been last updated
        /// </summary>
        public DateTime UpdatedDate { get; set; }

        /// <summary>
        /// User who has created this record
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// User who last updated this record
        /// </summary>
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
