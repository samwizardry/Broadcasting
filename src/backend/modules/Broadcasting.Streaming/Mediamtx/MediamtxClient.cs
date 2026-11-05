using Broadcasting.SharedKernel.Http;

namespace Broadcasting.Streaming.Mediamtx;

internal class MediamtxClient
{
    private readonly HttpClient _client;

    public MediamtxClient(HttpClient client)
    {
        _client = client;
    }

    public Task<ApiResponse<Unit, MediamtxError>> AddPathAsync(AddPathRequest req, string path, CancellationToken ct = default)
    {
        return _client.PostAsync<AddPathRequest, Unit, MediamtxError>($"/v3/config/paths/add/{path}", req, ct);
    }

    public Task<ApiResponse<Unit, MediamtxError>> ReplacePathAsync(AddPathRequest req, string path, CancellationToken ct = default)
    {
        return _client.PostAsync<AddPathRequest, Unit, MediamtxError>($"/v3/config/paths/replace/{path}", req, ct);
    }

    public Task<ApiResponse<Unit, MediamtxError>> DeletePathAsync(string path, CancellationToken ct = default)
    {
        return _client.DeleteAsync<Unit, MediamtxError>($"/v3/config/paths/delete/{path}", ct);
    }
}
