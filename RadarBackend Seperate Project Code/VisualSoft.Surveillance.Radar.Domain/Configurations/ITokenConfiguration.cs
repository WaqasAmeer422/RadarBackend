namespace VisualSoft.Surveillance.Radar.Domain.Configurations
{
    public interface ITokenConfiguration
    {
        public string Issuer { get; }
        public string Audience { get; }
        public string Key { get; }
    }
}
