using OneOf;
using VisualSoft.Surveillance.Radar.Domain.Utils;
using VisualSoft.Surveillance.Radar.Services.Models;

namespace VisualSoft.Surveillance.Radar.Services.Handlers
{
    public interface ITransactionBaseService
    {
        //Task<OneOf<AnprGeneratedData, ValidationResult>> ProcessData(object model);
        //Task<OneOf<RadarGeneratedData, ValidationResult>> ProcessData(object model);
        Task<OneOf<IGenertedData, ValidationResult>> ProcessData(object model);

    }
}
