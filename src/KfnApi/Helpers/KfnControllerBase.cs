using KfnApi.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace KfnApi.Helpers;

public class KfnControllerBase : ControllerBase
{
    protected IActionResult Failure(Error failure)
    {
        return new ObjectResult(failure)
        {
            StatusCode = failure.HttpCode
        };
    }

    protected IActionResult Success(object? value, int httpCode)
    {
        return new ObjectResult(value)
        {
            StatusCode = httpCode
        };
    }
}
