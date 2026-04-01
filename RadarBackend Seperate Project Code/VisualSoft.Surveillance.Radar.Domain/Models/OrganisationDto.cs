using System.Text.Json.Serialization;
using static VisualSoft.Surveillance.Radar.Domain.Models.OrganisationsWithPermissionsDto.OrganisationPermissionsDto;

namespace VisualSoft.Surveillance.Radar.Domain.Models
{
    public class OrganisationDto : BaseDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }


        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("phone_no")]
        public string PhoneNo { get; set; }

        [JsonPropertyName("primary_contact")]
        public string PrimaryContact { get; set; }

        [JsonPropertyName("secondary_contact")]
        public string SecondaryContact { get; set; }

        [JsonPropertyName("website")]
        public string Website { get; set; }

        [JsonPropertyName("expiry_date")]
        public DateTime? ExpiryDate { get; set; }

        [JsonPropertyName("access_rights")]
        public IEnumerable<KeyValues>? AccessRights { get; set; }
    }


    public class OrganisationsWithPermissionsDto
    {
        [JsonPropertyName("organisations")]
        public IEnumerable<OrganisationDto> Organisations { get; set; }

        [JsonPropertyName("access_rights")]
        public IEnumerable<OrganisationPermissionsDto> AccessRights { get; set; }

        public class OrganisationPermissionsDto
        {
            [JsonPropertyName("organisation_id")]
            public Guid OrganisationId { get; set; }
            [JsonPropertyName("access_rights")]
            public IEnumerable<KeyValues> AccessRights { get; set; }

            public class KeyValues
            {
                [JsonPropertyName("key")]
                public string Key { get; set; }
                [JsonPropertyName("value")]
                public string Value { get; set; }
            }
        }

    }
}
