using System.Net;
using System.Net.Http.Json;

using Broadcasting.Contracts.Responses;

namespace Broadcasting.Blazor.Client.HttpClients;

public static class HttpClientExtensions
{
    public static Task<ApiResponse<TResponse, ProblemDetails>> GetAsync<TResponse>(
        this HttpClient client,
        string uri,
        CancellationToken cancellationToken = default)
    {
        return SendRequestAsync<TResponse>(() => client.GetAsync(uri, cancellationToken), cancellationToken);
    }

    public static Task<ApiResponse<TResponse, ProblemDetails>> PostAsync<TRequest, TResponse>(
        this HttpClient client,
        string uri,
        TRequest body,
        CancellationToken cancellationToken = default)
    {
        return SendRequestAsync<TResponse>(() => client.PostAsJsonAsync(uri, body, cancellationToken), cancellationToken);
    }

    public static Task<ApiResponse<TResponse, ProblemDetails>> DeleteAsync<TResponse>(
        this HttpClient client,
        string uri,
        CancellationToken cancellationToken = default)
    {
        return SendRequestAsync<TResponse>(() => client.DeleteAsync(uri, cancellationToken), cancellationToken);
    }

    public static Task<ApiResponse<TResponse, ProblemDetails>> PutAsync<TRequest, TResponse>(
        this HttpClient client,
        string uri,
        TRequest body,
        CancellationToken cancellationToken = default)
    {
        return SendRequestAsync<TResponse>(() => client.PutAsJsonAsync(uri, body, cancellationToken), cancellationToken);
    }

    public static async Task<ApiResponse<TResponse, ProblemDetails>> SendRequestAsync<TResponse>(
        Func<Task<HttpResponseMessage>> request,
        CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;

        try
        {
            response = await request();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return ApiResponse<TResponse, ProblemDetails>.Failure(new ProblemDetails()
            {
                Title = "Request canceled.",
                Detail = "The operation was canceled."
            }, 0);
        }
        catch (Exception)
        {
            return ApiResponse<TResponse, ProblemDetails>.Failure(new ProblemDetails()
            {
                Status = (int)HttpStatusCode.ServiceUnavailable,
                Title = "Connection failed.",
                Detail = "Network Link Severed: Unable to reach server."
            }, HttpStatusCode.ServiceUnavailable);
        }

        using (response)
        {
            if (response.IsSuccessStatusCode)
            {
                if (typeof(TResponse) == typeof(Unit))
                {
                    return ApiResponse<TResponse, ProblemDetails>.Success(response.StatusCode);
                }

                var result = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
                return ApiResponse<TResponse, ProblemDetails>.Success(result!, response.StatusCode);
            }

            ProblemDetails? problem = null;

            try
            {
                problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);
            }
            catch
            {
                problem = new ProblemDetails
                {
                    Status = (int)response.StatusCode,
                    Title = response.ReasonPhrase ?? ProblemDetails.DefaultTitleValue,
                    Detail = $"Server returned HTTP {(int)response.StatusCode} ({response.ReasonPhrase ?? response.StatusCode.ToString()})."
                };
            }

            return ApiResponse<TResponse, ProblemDetails>.Failure(problem, response.StatusCode);
        }
    }
}
