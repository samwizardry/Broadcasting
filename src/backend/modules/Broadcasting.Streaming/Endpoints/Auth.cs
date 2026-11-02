using System.Net.Http.Json;

using Broadcasting.Streaming.Data;

using FastEndpoints;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Broadcasting.Streaming.Endpoints;

internal record AuthRequest(
    string User,
    string Password,
    string Token,
    string Ip,
    string Action,
    string Path,
    string Protocol,
    string Id,
    string Query,
    string UserAgent);

internal record MediamtxError(string Error, string Status);

internal class Auth : Endpoint<AuthRequest>
{
    private readonly StreamingDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<Auth> _logger;

    public Auth(StreamingDbContext context, IHttpClientFactory httpClientFactory,
        ILogger<Auth> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("auth");
        Group<Streaming>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(AuthRequest req, CancellationToken ct)
    {
        if (req.Action == "publish")
        {
            var secretKey = req.Path.Replace("live/", "");
            var streamer = await _context.Streamers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.StreamSecretKey == secretKey, ct);

            if (streamer is null)
            {
                await Send.UnauthorizedAsync(cancellation: ct);
                return;
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"http://localhost:9997/v3/config/paths/add/{streamer.ChannelSlug}",
                new { Source = $"rtsp://127.0.0.1:8554/{req.Path}" }, ct);

            if (response.IsSuccessStatusCode)
            {
                await Send.OkAsync(cancellation: ct);
                return;
            }
            else
            {
                MediamtxError error = (await response.Content.ReadFromJsonAsync<MediamtxError>())!;
                _logger.LogError("Cannot create stream path {Error}", error.Error);

                await Send.UnauthorizedAsync(cancellation: ct);
                return;
            }
        }

        if (req.Action == "read")
        {
            await Send.OkAsync(cancellation: ct);
            return;
        }

        await Send.UnauthorizedAsync(cancellation: ct);
    }

    private async Task<bool> TryAddPathAsync(string slug, string path, CancellationToken cancellationToken)
    {
        return false;
    }
}
