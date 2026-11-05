using Broadcasting.Contracts.Streaming;
using Broadcasting.Streaming.Data;
using Broadcasting.Streaming.Options;

using FastEndpoints;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Broadcasting.Streaming.Endpoints;

internal class GetChannelBySlug : EndpointWithoutRequest<StreamChannelResponse>
{
    private readonly StreamingDbContext _context;
    private readonly StreamingOptions _options;

    public GetChannelBySlug(StreamingDbContext context,
        IOptions<StreamingOptions> options)
    {
        _context = context;
        _options = options.Value;
    }

    public override void Configure()
    {
        Get("channels/{slug}");
        Group<Streaming>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string slug = Route<string>("slug", true)!;

        var channel = await _context.StreamChannels.AsNoTracking()
            .Where(x => x.ChannelSlug == slug)
            .Select(x => new StreamChannelResponse()
            {
                Username = x.Username,
                ChannelSlug = x.ChannelSlug,
                HlsAddress = $"{_options.HlsAddress}/live/{x.ChannelSlug}",
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
