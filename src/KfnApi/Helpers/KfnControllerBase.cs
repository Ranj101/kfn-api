using KfnApi.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace KfnApi.Helpers;

public class KfnControllerBase : ControllerBase
{
    protected IActionResult SuccessResponse(object? value, int httpCode)
    {
        return new ObjectResult(value)
        {
            StatusCode = httpCode
        };
    }

    protected IActionResult ErrorResponse(Error error)
    {
        return new ObjectResult(error)
        {
            StatusCode = error.HttpCode
        };
    }

    protected IActionResult NotFoundResponse()
    {
        var error = new Error
        {
            HttpCode = StatusCodes.Status404NotFound,
            Title = "Resource Not Found"
        };

        return new ObjectResult(error)
        {
            StatusCode = error.HttpCode
        };
    }
}
