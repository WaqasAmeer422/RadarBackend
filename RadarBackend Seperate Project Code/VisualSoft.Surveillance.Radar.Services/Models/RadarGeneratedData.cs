using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Services.Models
{
    public class RadarGeneratedData : IGenertedData
    {
        public RadarTransactionDto Transaction { get; set; }
     
    }
    public interface IGenertedData { }
}