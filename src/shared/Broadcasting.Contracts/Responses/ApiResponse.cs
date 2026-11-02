using System.Net;

namespace Broadcasting.Contracts.Responses;

public class ApiResponse<TError>
{
    protected internal  ApiResponse() { }

    public bool Succeeded { get; init; }

    public HttpStatusCode StatusCode { get; init; }

    public TError? Error { get; init; }

    public static ApiResponse<TError> Success(HttpStatusCode status) =>
        new()
        {
            Succeeded = true,
            StatusCode = status
        };

    public static ApiResponse<TError> Failure(TError? error, HttpStatusCode status) =>
        new()
        {
            StatusCode = status,
            Error = error
        };
}

public class ApiResponse<TResult, TError> : ApiResponse<TError>
{
    protected internal ApiResponse() { }

    public TResult? Result { get; init; }

    public static new ApiResponse<TResult, TError> Success(HttpStatusCode status) =>
        new()
        {
            Succeeded = true,
            StatusCode = status
        };

    public static ApiResponse<TResult, TError> Success(TResult result, HttpStatusCode status) =>
        new()
        {
            Succeeded = true,
            StatusCode = status,
            Result = result
        };

    public static new ApiResponse<TResult, TError> Failure(TError? error, HttpStatusCode status) =>
        new()
        {
            StatusCode = status,
            Error = error
        };
}
