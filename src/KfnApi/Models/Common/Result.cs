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

    public static Result<T> FailureResult(Error error)
    {
        return new Result<T>(error);
    }

    public bool IsSuccess()
    {
        return Value != null;
    }

    public bool IsFailure()
    {
        return Error != null;
    }
}

public class Error
{
    public int HttpCode { get; set; }
    public string? Title { get; set; }
    public string? Detail { get; set; }
}
