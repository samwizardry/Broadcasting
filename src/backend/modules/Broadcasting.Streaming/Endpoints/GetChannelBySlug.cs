using Broadcasting.Contracts.Streaming;
using Broadcasting.Streaming.Data;

using FastEndpoints;

using Microsoft.EntityFrameworkCore;

namespace Broadcasting.Streaming.Endpoints;

internal class GetChannelBySlug : EndpointWithoutRequest<StreamChannelResponse>
{
    private readonly StreamingDbContext _context;

    public GetChannelBySlug(StreamingDbContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Get("channels/{slug}");
        Group<Streaming>();
        AllowAnonymous();
    }

    // TODO: cfg
    public override async Task HandleAsync(CancellationToken ct)
    {
        string slug = Route<string>("slug", true)!;

        var channel = await _context.StreamChannels.AsNoTracking()
            .Where(x => x.ChannelSlug == slug)
            .Select(x => new StreamChannelResponse()
            {
                Username = x.Username,
                RtspUrl = $"rtsp://127.0.0.1:8554/{x.ChannelSlug}",
                IsLive = x.IsLive
            })
            .FirstOrDefaultAsync(ct);

        if (channel is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(channel, ct);
    }
}
