using System.Diagnostics;
using ILogger = Serilog.ILogger;
namespace VisualSoft.Surveillance.Radar.Api.Middlewares
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestTimingMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context); // Call the next middleware in the pipeline
            }
            finally
            {
                stopwatch.Stop();
                var elapsedMs = stopwatch.Elapsed.TotalMilliseconds;

                // Log the time taken. You can customize the log message and level.
                _logger.Debug(
                    "Request {Method} {Path} completed in {ElapsedMs:0.0000} ms with status {StatusCode}",
                    context.Request.Method,
                    context.Request.Path,
                    elapsedMs,
                    context.Response.StatusCode
                );
            }
        }
    }
}
