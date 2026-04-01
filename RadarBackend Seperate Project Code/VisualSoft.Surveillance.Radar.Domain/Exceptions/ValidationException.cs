using VisualSoft.Surveillance.Radar.Domain.Utils;

namespace VisualSoft.Surveillance.Radar.Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException()
        {
            Errors = new List<ValidationError>();
        }

        public ValidationException(IList<ValidationError> errors)
        {
            Errors = errors.ToList();
        }

        public List<ValidationError> Errors { get; private set; }
    }
}
