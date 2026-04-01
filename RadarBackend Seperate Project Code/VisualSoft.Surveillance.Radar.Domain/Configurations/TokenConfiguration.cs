using Microsoft.Extensions.Configuration;

namespace VisualSoft.Surveillance.Radar.Domain.Configurations
{
    public class TokenConfiguration : ITokenConfiguration
    {
        private IConfiguration Configuration { get; }

        public TokenConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public string Issuer
        {
            get
            {
                return Configuration["Tokens:Issuer"];
            }
        }
        public string Audience
        {
            get
            {
                return Configuration["Tokens:Audience"];
            }
        }
        public string Key
        {
            get
            {
                return Configuration["Tokens:Key"];
            }
        }
    }
}
