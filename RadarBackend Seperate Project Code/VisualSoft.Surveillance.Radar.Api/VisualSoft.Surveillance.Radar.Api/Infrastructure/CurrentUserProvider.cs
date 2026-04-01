using Microsoft.AspNetCore.Authentication.JwtBearer;
using VisualSoft.Surveillance.Radar.Domain;
using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Api.Infrastructure
{
    public class CurrentUserProvider
    {
        public IUserIdentificationModel CurrentUser { get; private set; }

        public void BuildUp(TokenValidatedContext context)
        {
            if (CurrentUser != null)
            {
                throw new InvalidOperationException();
            }
            CurrentUser = new UserIdentificationModel();

            var claims = context.Principal.Claims;


            // Extract Id
            var userIdClaim = claims.FirstOrDefault(c => c.Type == Constants.JwtToken.USER_IDENTITY_CLAIM);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                CurrentUser.Id = userId;
            }
            else
            {
                // If user ID claim is missing or invalid, consider the user identification incomplete or invalid.
                // You might return null or throw an exception here depending on your application's tolerance.
                return;
            }

            // Extract OrganizationId
            var orgIdClaim = claims.FirstOrDefault(c => c.Type == Constants.JwtToken.USER_ORGANISATION_CLAIM);
            if (orgIdClaim != null && Guid.TryParse(orgIdClaim.Value, out Guid orgId))
            {
                CurrentUser.OrganizationId = orgId;
            }

            CurrentUser.Name = claims.FirstOrDefault(c => c.Type == Constants.JwtToken.USER_NAME_CLAIM)?.Value;
            CurrentUser.EmailAddress = claims.FirstOrDefault(c => c.Type == Constants.JwtToken.USER_EMAIL_CLAIM)?.Value;


            // Populate Roles (if roles are added as claims, e.g., ClaimTypes.Role)
            CurrentUser.Roles = claims
                .Where(c => c.Type == Constants.JwtToken.USER_ROLE_CLAIM)
                .Select(c => c.Value /* assuming Role has a Name property */ )
                .ToList();
            // If roles need more complex data (like Id), you'd need to fetch them from DB or store more in claims.
        }
    }
}
