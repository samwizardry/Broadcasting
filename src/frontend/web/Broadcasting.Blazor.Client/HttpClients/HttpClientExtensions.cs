using System.Net.Http.Json;

using Broadcasting.Contracts.Responses;

namespace Broadcasting.Blazor.Client.HttpClients;

public readonly struct Unit;

public static class HttpClientExtensions
{
    public static async Task<ApiResponse<TResponse>> GetAsync<TResponse>(
        this HttpClient client,
        string url,
        CancellationToken ct = default)
    {
        return await SendRequestAsync<TResponse>(() => client.GetAsync(url, ct), ct);
    }

    public static async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(
        this HttpClient client,
        string url,
        TRequest body,
        CancellationToken ct = default)
    {
        return await SendRequestAsync<TResponse>(() => client.PostAsJsonAsync(url, body, ct), ct);
    }

    public static async Task<ApiResponse<TResponse>> PostAsync<TResponse>(
        this HttpClient client,
        string url,
        CancellationToken ct = default)
    {
        return await SendRequestAsync<TResponse>(() =>
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            return client.SendAsync(request);
        }, ct);
    }

    public static async Task<ApiResponse<TResponse>> SendRequestAsync<TResponse>(
        Func<Task<HttpResponseMessage>> request,
        CancellationToken ct = default)
    {
        using var response = await request();

        if (response.IsSuccessStatusCode)
        {
            if (typeof(TResponse) == typeof(Unit))
            {
                return ApiResponse<TResponse>.Success(response.StatusCode);
            }

            var result = await response.Content.ReadFromJsonAsync<TResponse>(ct);
            return ApiResponse<TResponse>.Success(result!, response.StatusCode);
        }

        ProblemDetails? problem = null;

        try
        {
            problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(ct);
        }
        catch
        {
            problem = new ProblemDetails
            {
                Status = (int)response.StatusCode,
                Title = response.ReasonPhrase,
                Detail = await response.Content.ReadAsStringAsync(ct)
            };
        }

        return ApiResponse<TResponse>.Failure(problem);
    }
}
