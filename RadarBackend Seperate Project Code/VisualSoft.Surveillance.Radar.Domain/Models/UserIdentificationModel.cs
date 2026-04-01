namespace VisualSoft.Surveillance.Radar.Domain.Models
{
    public class UserIdentificationModel : IUserIdentificationModel
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public List<string> Roles { get; set; }

        public bool IsSuperUser
        {
            get
            {
                return Roles.Any(x => x == Constants.Roles.SYSADMIN || x == Constants.Roles.SERVICE);
            }
        }

        public bool IsAdministrator
        {
            get
            {
                return Roles.Any(x => x == Constants.Roles.SYSADMIN || x == Constants.Roles.ADMIN);
            }
        }
    }
}
