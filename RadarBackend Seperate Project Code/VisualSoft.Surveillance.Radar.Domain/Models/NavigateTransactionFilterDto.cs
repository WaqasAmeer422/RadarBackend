using VisualSoft.Surveillance.Radar.Domain.Enums;

namespace VisualSoft.Surveillance.Radar.Domain.Models
{
    public class NavigateTransactionFilterDto
    {
        public DateTime? FromDate { get; set; }
        public string? Location { get; set; }
        public NavigationDirection NavigationDirection { get; set; }
    }
}
