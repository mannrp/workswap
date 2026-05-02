using System.Net;

namespace workswap.Common;

/// <summary>
/// Represents the result of an operation, providing a unified way to handle success and failure
/// without relying on exceptions for expected business logic outcomes.
/// </summary>
/// <typeparam name="T">The type of value returned on success.</typeparam>
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public HttpStatusCode StatusCode { get; }

    private Result(bool isSuccess, T? value, string? error, HttpStatusCode statusCode)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        StatusCode = statusCode;
    }

    public static Result<T> Success(T value) => new(true, value, null, HttpStatusCode.OK);
    
    public static Result<T> Failure(string error, HttpStatusCode statusCode = HttpStatusCode.BadRequest) 
        => new(false, default, error, statusCode);

    public static Result<T> NotFound(string message = "Resource not found") 
        => Failure(message, HttpStatusCode.NotFound);

    public static Result<T> Forbidden(string message = "You do not have permission to perform this action") 
        => Failure(message, HttpStatusCode.Forbidden);

    public static Result<T> Unauthorized(string message = "Unauthorized") 
        => Failure(message, HttpStatusCode.Unauthorized);
}

/// <summary>
/// Represents a result of an operation with no return value.
/// </summary>
public class Result : Result<object?>
{
    private Result(bool isSuccess, string? error, HttpStatusCode statusCode) 
        : base(isSuccess, null, error, statusCode) { }

    public static Result Success() => new(true, null, HttpStatusCode.OK);
    
    public new static Result Failure(string error, HttpStatusCode statusCode = HttpStatusCode.BadRequest) 
        => new(false, error, statusCode);

    public new static Result NotFound(string message = "Resource not found") 
        => new(false, message, HttpStatusCode.NotFound);

    public new static Result Forbidden(string message = "You do not have permission to perform this action") 
        => new(false, message, HttpStatusCode.Forbidden);
}
