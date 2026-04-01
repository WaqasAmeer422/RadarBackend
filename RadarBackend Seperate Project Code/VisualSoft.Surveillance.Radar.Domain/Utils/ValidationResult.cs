namespace VisualSoft.Surveillance.Radar.Domain.Utils
{
    public class ValidationResult
    {
        public static readonly ValidationResult Success = new ValidationResult();

        public IList<ValidationError> Errors { get; set; } = new ValidationError[0];

        public bool Failed => Errors.Any();

        public ValidationResult()
        {

        }

        public static ValidationResult Error(string field, string message)
        {
            return new ValidationResult()
            {
                Errors = new[] { new ValidationError(field, message) }
            };
        }
    }
}
