using Broadcasting.SharedKernel.Http;

namespace Broadcasting.Streaming.Http;

internal class MediamtxClient
{
    private readonly HttpClient _client;

    public MediamtxClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<ApiResponse<MediamtxError>> AddPathAsync(AddPathRequest req, string path, CancellationToken ct = default)
    {
        return await _client.PostAsync<AddPathRequest, Unit, MediamtxError>($"/v3/config/paths/add/{path}", req, ct);
    }
}
