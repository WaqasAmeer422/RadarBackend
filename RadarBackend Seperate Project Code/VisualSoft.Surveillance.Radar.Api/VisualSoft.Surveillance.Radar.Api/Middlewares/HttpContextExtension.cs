using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using VisualSoft.Surveillance.Radar.Api.Models;

namespace VisualSoft.Surveillance.Radar.Api.Middlewares
{
    public static class HttpContextExtension
    {
        public static async Task WriteErrorResponse(this HttpContext context, HttpStatusCode statusCode, ErrorResponse errorResponse)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse), Encoding.UTF8);
        }
    }
}
