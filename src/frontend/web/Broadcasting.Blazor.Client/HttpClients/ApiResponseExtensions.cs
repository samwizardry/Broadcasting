using Broadcasting.Contracts.Responses;

namespace Broadcasting.Blazor.Client.HttpClients;

public static class ApiResponseExtensions
{
    public static IEnumerable<string> GetErrors(this ApiResponse<ProblemDetails> response)
    {
        if (response.Error is null)
        {
            yield return response.StatusCode != 0
                ? $"Server returned HTTP {(int)response.StatusCode} ({response.StatusCode})"
                : "Unknown error";
            yield break;
        }

        if (response.Error is not null)
        {
            if (response.Error.Errors?.Count > 0)
            {
                foreach (var error in response.Error.Errors)
                {
                    yield return error.Reason;
                }
                yield break;
            }

            if (!string.IsNullOrWhiteSpace(response.Error.Detail))
            {
                yield return response.Error.Detail;
                yield break;
            }

            if (!string.IsNullOrWhiteSpace(response.Error.Title))
            {
                yield return response.Error.Title;
                yield break;
            }
        }

        yield return response.StatusCode != 0
            ? $"Server returned HTTP {(int)response.StatusCode} ({response.StatusCode})"
            : "Unknown error";
        yield break;
    }

    public static string GetErrorMessage(this ApiResponse<ProblemDetails> response, char separator)
    {
        return string.Join(separator, response.GetErrors());
    }

    public static string GetErrorMessage(this ApiResponse<ProblemDetails> response, string separator)
    {
        return string.Join(separator, response.GetErrors());
    }
}
