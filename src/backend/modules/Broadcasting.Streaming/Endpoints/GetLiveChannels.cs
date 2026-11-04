using Broadcasting.Contracts.Streaming;
using Broadcasting.Streaming.Data;

using FastEndpoints;

using Microsoft.EntityFrameworkCore;

namespace Broadcasting.Streaming.Endpoints;

internal class GetLiveChannels : EndpointWithoutRequest<List<StreamChannelResponse>>
{
    private readonly StreamingDbContext _context;

    public GetLiveChannels(StreamingDbContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Get("channels");
        Group<Streaming>();
        AllowAnonymous();
    }

    // TODO: cfg
    public override async Task HandleAsync(CancellationToken ct)
    {
        var channels = await _context.StreamChannels.AsNoTracking()
            .Where(x => x.IsLive)
            .Select(x => new StreamChannelResponse()
            {
                Username = x.Username,
                ChannelSlug = x.ChannelSlug,
                HlsAddress = $"http://127.0.0.1:8888/live/{x.ChannelSlug}",
                IsLive = x.IsLive
            })
            .ToListAsync(ct);

        await Send.OkAsync(channels, ct);
    }
}
