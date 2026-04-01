namespace VisualSoft.Surveillance.Radar.Domain.Models
{
    public interface IUserIdentificationModel
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public bool IsSuperUser { get; }
        public bool IsAdministrator { get; }
        public List<string> Roles { get; set; }

    }
}
