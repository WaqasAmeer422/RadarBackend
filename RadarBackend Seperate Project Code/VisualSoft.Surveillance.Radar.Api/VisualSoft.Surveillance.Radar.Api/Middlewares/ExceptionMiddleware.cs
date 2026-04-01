using System.Net;
using VisualSoft.Surveillance.Radar.Api.Models;
using VisualSoft.Surveillance.Radar.Domain;
using VisualSoft.Surveillance.Radar.Domain.Enums;
using VisualSoft.Surveillance.Radar.Domain.Exceptions;
using ILogger = Serilog.ILogger;

namespace VisualSoft.Surveillance.Radar.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (HttpException httpException)
            {
                _logger.Error(httpException, Constants.Errors.ERROR_EXCEPTION_OCCURED);
                var content = new ErrorResponse(ErrorType.Error.ToString(), httpException.StatusCode, httpException.Message, httpContext.Request.Path);

                var exception = httpException.InnerException as ValidationException;
                if (exception != null)
                    content.Errors.AddRange(exception.Errors);

                await httpContext.WriteErrorResponse(httpException.StatusCode, content);
            }
            catch (ValidationException validationException)
            {
                if (validationException.Errors == null)
                {
                    _logger.Error(validationException, Constants.Errors.ERROR_EXCEPTION_OCCURED);
                    return;
                }
                var message = validationException.Errors.Select(x => x.Message).ToList();
                _logger.Error(validationException, string.Join(", ", message));
                var content = new ErrorResponse
                {
                    ErrorCode = HttpStatusCode.BadRequest,
                    ErrorType = ErrorType.Error.ToString(),
                    Errors = validationException.Errors,
                    RequestUrl = httpContext.Request.Path
                };
                await httpContext.WriteErrorResponse(HttpStatusCode.BadRequest, content);
            }
            catch (Exception exeption)
            {
                _logger.Error(exeption, Constants.Errors.ERROR_EXCEPTION_OCCURED);
                var content = new ErrorResponse(ErrorType.Error.ToString(), HttpStatusCode.InternalServerError, Constants.Errors.ERROR_EXCEPTION_OCCURED, httpContext.Request.Path);
                await httpContext.WriteErrorResponse(HttpStatusCode.InternalServerError, content);
            }
        }
    }
}
