using Broadcasting.Streaming.Data;
using Broadcasting.Streaming.Mediamtx;
using Broadcasting.Streaming.Options;

using FastEndpoints;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Broadcasting.Streaming.Endpoints.Hooks;

internal class RunOnOnline : EndpointWithoutRequest
{
    private readonly ILogger<RunOnOnline> _logger;
    private readonly StreamingDbContext _context;
    private readonly MediamtxClient _client;
    private readonly StreamingOptions _options;

    public RunOnOnline(ILogger<RunOnOnline> logger,
        StreamingDbContext context,
        MediamtxClient client,
        IOptions<StreamingOptions> options)
    {
        _logger = logger;
        _context = context;
        _client = client;
        _options = options.Value;
    }

    public override void Configure()
    {
        Get("run-on-online");
        Group<Streaming>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string path = Query<string>("path") ?? "";
        string sourceType = Query<string>("source_type") ?? "";

        _logger.LogInformation("Stream '{Path}' is now online.", path);

        var sk = path.Replace("live/", "");
        var channel = await _context.StreamChannels
            .Where(x => x.StreamSecretKey == sk)
            .FirstOrDefaultAsync(ct);

        if (channel is null)
        {
            _logger.LogInformation("Stream channel with path '{Path}' not found.", path);
            await Send.NotFoundAsync(ct);
            return;
        }

        var response = await _client.ReplacePathAsync(new(Source: $"{_options.RtspAddress}/live/{channel.StreamSecretKey}"), $"live/{channel.ChannelSlug}");
        if (response.Succeeded)
        {
            _logger.LogInformation("Added channel path '{Path}'.", $"live/{channel.ChannelSlug}");
        }
        else
        {
            _logger.LogError("Failed to add channel path. {Error}", response.Error.Error);
        }

        channel.SetLiveStatus(true);
        await _context.SaveChangesAsync(ct);

        await Send.NoContentAsync(cancellation: ct);
        return;
    }
}
