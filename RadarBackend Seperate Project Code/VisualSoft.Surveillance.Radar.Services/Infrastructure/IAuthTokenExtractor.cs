namespace VisualSoft.Surveillance.Radar.Services.Infrastructure
{
    public interface IAuthTokenExtractor
    {
        Task<string?> Extract();
    }
}
