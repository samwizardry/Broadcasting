using Broadcasting.Contracts.Responses;
using Broadcasting.Contracts.Streaming;

namespace Broadcasting.Blazor.Client.HttpClients;

public class StreamingClient
{
    private readonly HttpClient _client;

    public StreamingClient(HttpClient client)
    {
        _client = client;
    }

    public Task<ApiResponse<StreamChannelResponse, ProblemDetails>> CreateChannelAsync(CreateChannelRequest req, CancellationToken ct = default)
    {
        return _client.PostAsync<CreateChannelRequest, StreamChannelResponse>("/api/streaming/channels", req, ct);
    }

    public Task<ApiResponse<Unit, ProblemDetails>> DeleteChannelAsync(string username, CancellationToken ct = default)
    {
        return _client.DeleteAsync<Unit>($"/api/streaming/channels/{username}", ct);
    }

    public Task<ApiResponse<List<StreamChannelResponse>, ProblemDetails>> GetLiveChannelsAsync(CancellationToken ct = default)
    {
        return _client.GetAsync<List<StreamChannelResponse>>("/api/streaming/channels", ct);
    }

    public Task<ApiResponse<StreamChannelResponse, ProblemDetails>> GetChannelBySlugAsync(string channelSlug, CancellationToken ct = default)
    {
        return _client.GetAsync<StreamChannelResponse>($"/api/streaming/channels/{channelSlug}", ct);
    }

    public Task<ApiResponse<StreamSettingsResponse, ProblemDetails>> GetStreamSettingsAsync(string username, CancellationToken ct)
    {
        return _client.GetAsync<StreamSettingsResponse>($"/api/streaming/channels/{username}/settings", ct);
    }
}
