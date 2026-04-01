namespace VisualSoft.Surveillance.Radar.Domain.Models
{
    public class LookupDto : BaseDto
    {
        public Guid Id { get; set; }
        public string Correlation { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string ParentCode { get; set; }
        public string TargetTable { get; set; }
        public string TargetColumn { get; set; }
    }
}
