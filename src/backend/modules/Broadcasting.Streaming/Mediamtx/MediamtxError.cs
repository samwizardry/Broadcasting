using System.Net.Http.Json;

using Broadcasting.SharedKernel.Http;

namespace Broadcasting.Streaming.Mediamtx;

internal class MediamtxError : IError<MediamtxError>
{
    public string Status { get; init; } = default!;

    public string Error { get; init; } = default!;

    public static MediamtxError Create(int statusCode, string title, string detail) =>
        new()
        {
            Status = title,
            Error = detail
        };

    public static Task<MediamtxError?> CreateAsync(HttpResponseMessage response, CancellationToken ct = default) =>
        response.Content.ReadFromJsonAsync<MediamtxError>(ct);
}
