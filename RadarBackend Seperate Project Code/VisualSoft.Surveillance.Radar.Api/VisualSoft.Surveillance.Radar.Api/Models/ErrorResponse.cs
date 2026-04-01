using System.Net;
using System.Text.Json.Serialization;
using VisualSoft.Surveillance.Radar.Domain.Utils;

namespace VisualSoft.Surveillance.Radar.Api.Models
{
    public class ErrorResponse
    {
        /// <summary>
        /// Error Code (http status code)
        /// </summary>
        [JsonPropertyName("error_code")]
        public HttpStatusCode ErrorCode { get; set; }

        /// <summary>
        /// List of ValidationErrors
        /// </summary>
        [JsonPropertyName("errors")]
        public List<ValidationError> Errors { get; set; } = new List<ValidationError>();

        /// <summary>
        /// Error Type
        /// </summary>
        [JsonPropertyName("error_type")]
        public string ErrorType { get; set; } = string.Empty;

        /// <summary>
        /// Request URL during whihc error occured
        /// </summary>
        [JsonPropertyName("request_url")]
        public string RequestUrl { get; set; } = string.Empty;

        public ErrorResponse()
        {

        }


        public ErrorResponse(HttpStatusCode code, IEnumerable<ValidationError> errors)
        {
            ErrorCode = code;
            Errors = errors.ToList();
        }

        public ErrorResponse(string type, HttpStatusCode code, string message, string requestUrl)
        {
            var errorList = new List<ValidationError>();
            errorList.Add(new ValidationError(string.Empty, message));
            ErrorType = type;
            ErrorCode = code;
            Errors = errorList;
            RequestUrl = requestUrl;
        }
    }
}

