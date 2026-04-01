using VisualSoft.Surveillance.Radar.Domain.Enums;

namespace VisualSoft.Surveillance.Radar.Api.Models
{
    public class NavigateTransactionFilterRequest
    {
        /// <summary>
        /// From date
        /// </summary>
        public DateTime? FromDate { get; set; }
        /// <summary>
        /// Locations
        /// </summary>
        public string? Location { get; set; }
        /// <summary>
        /// Navigation Direction
        /// </summary>
        /// <remarks>
        /// 0 - Latest, 1 - Next, 2 - Previous
        /// </remarks>
        public NavigationDirection NavigationDirection { get; set; }
    }
}
