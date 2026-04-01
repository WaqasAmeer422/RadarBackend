using System.Net;
using System.Runtime.Serialization;

namespace VisualSoft.Surveillance.Radar.Api.Middlewares
{
    [Serializable]
    internal class HttpException : Exception
    {
        public HttpException()
        {
        }

        public HttpException(string? message) : base(message)
        {
        }

        public HttpException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected HttpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public HttpStatusCode StatusCode { get; internal set; }
    }
}
