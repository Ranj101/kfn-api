using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KfnApi.Helpers;

public class UnhandledExceptionFilter : IExceptionFilter
{
    private readonly ILogger<UnhandledExceptionFilter> _logger;

    public UnhandledExceptionFilter(ILogger<UnhandledExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogCritical(context.Exception, "An unhandled exception occured");

        var error = new
        {
            Title = "An unhandled error occured.",
            Detail = context.Exception.Message,
            Meta = new
            {
                Type = context.Exception.GetType().Name,
                TraceId = context.HttpContext.TraceIdentifier,
                StackTrace = context.Exception.StackTrace
            }
        };

        context.Result = new ObjectResult(error)
        {
            StatusCode = 500
        };
    }
}
