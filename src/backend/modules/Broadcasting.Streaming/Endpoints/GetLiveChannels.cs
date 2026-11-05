using Broadcasting.Contracts.Streaming;
using Broadcasting.Streaming.Data;
using Broadcasting.Streaming.Options;

using FastEndpoints;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Broadcasting.Streaming.Endpoints;

internal class GetLiveChannels : EndpointWithoutRequest<List<StreamChannelResponse>>
{
    private readonly StreamingDbContext _context;
    private readonly StreamingOptions _options;

    public GetLiveChannels(StreamingDbContext context,
        IOptions<StreamingOptions> options)
    {
        _context = context;
        _options = options.Value;
    }

    public override void Configure()
    {
        Get("channels");
        Group<Streaming>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var channels = await _context.StreamChannels.AsNoTracking()
            .Where(x => x.IsLive)
            .Select(x => new StreamChannelResponse()
            {
                Username = x.Username,
                ChannelSlug = x.ChannelSlug,
                HlsAddress = $"{_options.HlsAddress}/live/{x.ChannelSlug}",
                IsLive = x.IsLive
            })
            .ToListAsync(ct);

        await Send.OkAsync(channels, ct);
    }
}
