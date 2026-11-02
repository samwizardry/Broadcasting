using Broadcasting.Streaming.Data;

using FastEndpoints;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Broadcasting.Streaming.Endpoints.Hooks;

internal class RunOnOffline : EndpointWithoutRequest
{
    private readonly ILogger<RunOnOffline> _logger;
    private readonly StreamingDbContext _context;

    public RunOnOffline(ILogger<RunOnOffline> logger,
        StreamingDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public override void Configure()
    {
        Get("run-on-offline");
        Group<Streaming>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string path = Query<string>("path") ?? "";
        string sourceType = Query<string>("source_type") ?? "";

        _logger.LogInformation("Stream '{Path}' is now offline.", path);

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

        channel.SetLiveStatus(false);
        await _context.SaveChangesAsync(ct);

        await Send.NoContentAsync(cancellation: ct);
        return;
    }
}
