using System.Net;

namespace Broadcasting.Contracts.Responses;

public class ApiResponse
{
    internal ApiResponse() { }

    public bool Succeeded { get; init; }

    public HttpStatusCode StatusCode { get; init; }

    public ProblemDetails? Problem { get; init; }

    public static ApiResponse Success(HttpStatusCode status) =>
        new ApiResponse { Succeeded = true, StatusCode = status };

    public static ApiResponse Failure(ProblemDetails? problem, HttpStatusCode status) =>
        new ApiResponse { StatusCode = status, Problem = problem };
}

public class ApiResponse<T> : ApiResponse
{
    internal ApiResponse() { }

    public T? Result { get; init; }

    public static new ApiResponse<T> Success(HttpStatusCode status) =>
        new ApiResponse<T> { Succeeded = true, StatusCode = status };

    public static ApiResponse<T> Success(T result, HttpStatusCode status) =>
        new ApiResponse<T> { Succeeded = true, StatusCode = status, Result = result };

    public static new ApiResponse<T> Failure(ProblemDetails? problem, HttpStatusCode status) =>
        new ApiResponse<T> { StatusCode = status, Problem = problem };
}

public static class ApiResponseExtensions
{
    public static IEnumerable<string> GetErrors(this ApiResponse response)
    {
        if (response.Problem is null)
        {
            yield return response.StatusCode != 0
                ? $"Server returned HTTP {(int)response.StatusCode} ({response.StatusCode})"
                : "Unknown error";
            yield break;
        }

        if (response.Problem is not null)
        {
            if (response.Problem.Errors?.Count > 0)
            {
                foreach (var error in response.Problem.Errors)
                {
                    yield return error.Reason;
                }
                yield break;
            }

            if (!string.IsNullOrWhiteSpace(response.Problem.Detail))
            {
                yield return response.Problem.Detail;
                yield break;
            }

            if (!string.IsNullOrWhiteSpace(response.Problem.Title))
            {
                yield return response.Problem.Title;
                yield break;
            }
        }

        yield return response.StatusCode != 0
            ? $"Server returned HTTP {(int)response.StatusCode} ({response.StatusCode})"
            : "Unknown error";
        yield break;
    }

    public static string GetErrorMessage(this ApiResponse response, char separator)
    {
        return string.Join(separator, response.GetErrors());
    }

    public static string GetErrorMessage(this ApiResponse response, string separator)
    {
        return string.Join(separator, response.GetErrors());
    }
}
