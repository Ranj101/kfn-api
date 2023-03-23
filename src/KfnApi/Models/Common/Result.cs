namespace KfnApi.Models.Common;

public class Result<T> where T : class
{
    public Result(T value, int httpCode)
    {
        Value = value;
        HttpCode = httpCode;
    }

    public Result(Error error)
    {
        Error = error;
    }

    public T? Value { get; }
    public int HttpCode { get; }
    public Error? Error { get; set; }

    public static Result<T> SuccessResult(T value, int httpCode)
    {
        return new Result<T>(value, httpCode);
    }

    public static Result<T> ErrorResult(Error error)
    {
        return new Result<T>(error);
    }

    public static Result<T> NotFoundResult()
    {
        return new Result<T>(new Error
        {
            HttpCode = StatusCodes.Status404NotFound,
            Title = "Resource Not Found"
        });
    }

    public static Result<T> StateErrorResult()
    {
        return new Result<T>(new Error
        {
            HttpCode = StatusCodes.Status422UnprocessableEntity,
            Title = "Update Not Permitted",
            Detail = "State update not permitted."
        });
    }

    public bool IsSuccess()
    {
        return Value != null;
    }
}

public class Error
{
    public int HttpCode { get; set; }
    public string? Title { get; set; }
    public string? Detail { get; set; }
}
