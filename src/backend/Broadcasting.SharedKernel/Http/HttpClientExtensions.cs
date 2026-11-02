using System.Net;
using System.Net.Http.Json;

namespace Broadcasting.SharedKernel.Http;

public static class HttpClientExtensions
{
    public static Task<ApiResponse<TResult, TError>> GetAsync<TResult, TError>(
        this HttpClient client,
        string uri,
        CancellationToken ct = default)
        where TError : IError<TError>
    {
        return SendRequestAsync<TResult, TError>(() => client.GetAsync(uri, ct), ct);
    }

    public static Task<ApiResponse<TResult, TError>> PostAsync<TRequest, TResult, TError>(
        this HttpClient client,
        string uri,
        TRequest body,
        CancellationToken ct = default)
        where TError : IError<TError>
    {
        return SendRequestAsync<TResult, TError>(() => client.PostAsJsonAsync(uri, body, ct), ct);
    }

    public static Task<ApiResponse<TResult, TError>> DeleteAsync<TResult, TError>(
        this HttpClient client,
        string uri,
        CancellationToken ct = default)
        where TError : IError<TError>
    {
        return SendRequestAsync<TResult, TError>(() => client.DeleteAsync(uri, ct), ct);
    }

    public static Task<ApiResponse<TResult, TError>> PutAsync<TRequest, TResult, TError>(
        this HttpClient client,
        string uri,
        TRequest body,
        CancellationToken ct = default)
        where TError : IError<TError>
    {
        return SendRequestAsync<TResult, TError>(() => client.PutAsJsonAsync(uri, body, ct), ct);
    }

    public static async Task<ApiResponse<TResult, TError>> SendRequestAsync<TResult, TError>(
        Func<Task<HttpResponseMessage>> request,
        CancellationToken ct = default)
        where TError : IError<TError>
    {
        try
        {
            using var response = await request();

            if (response.IsSuccessStatusCode)
            {
                if (typeof(TResult) == typeof(Unit))
                {
                    return ApiResponse<TResult, TError>.Success(response.StatusCode);
                }

                try
                {
                    var result = await response.Content.ReadFromJsonAsync<TResult>(ct);
                    return result is null
                        ? ApiResponse<TResult, TError>.Failure(
                            TError.Create((int)response.StatusCode, "Invalid response.", "Response body was null."),
                            response.StatusCode)
                        : ApiResponse<TResult, TError>.Success(result, response.StatusCode);
                }
                catch (Exception ex) when (!(ex is OperationCanceledException && ct.IsCancellationRequested))
                {
                    return ApiResponse<TResult, TError>.Failure(
                        TError.Create((int)response.StatusCode, "Deserialization failed.", ex.Message),
                        response.StatusCode);
                }
            }

            TError? error = default;

            try
            {
                error = await TError.CreateAsync(response, ct);
            }
            catch (Exception ex) when (!(ex is OperationCanceledException && ct.IsCancellationRequested))
            {
            }

            error ??= TError.Create(
                (int)response.StatusCode,
                response.ReasonPhrase ?? "One or more validation errors occurred.",
                $"Server returned HTTP {(int)response.StatusCode} ({response.ReasonPhrase ?? response.StatusCode.ToString()}).");

            return ApiResponse<TResult, TError>.Failure(error, response.StatusCode);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return ApiResponse<TResult, TError>.Failure(
                TError.Create(0, "Request canceled.", "The operation was canceled."),
                0);
        }
        catch (Exception ex)
        {
            return ApiResponse<TResult, TError>.Failure(
                TError.Create((int)HttpStatusCode.ServiceUnavailable, "Connection failed.", $"Unable to reach server: {ex.Message}"),
                HttpStatusCode.ServiceUnavailable);
        }
    }
}
