using Broadcasting.Contracts.Streaming;
using Broadcasting.SharedKernel.Abstractions;
using Broadcasting.Streaming.Data;
using Broadcasting.Streaming.Domain;

using FastEndpoints;

using Microsoft.Extensions.Logging;

namespace Broadcasting.Streaming.Endpoints;

internal class CreateChannel : Endpoint<CreateChannelRequest, StreamChannelResponse>
{
    private readonly ILogger<CreateChannel> _logger;
    private readonly StreamingDbContext _context;
    private readonly ISlugifier _slugifier;

    public CreateChannel(ILogger<CreateChannel> logger,
        StreamingDbContext context,
        ISlugifier slugifier)
    {
        _logger = logger;
        _context = context;
        _slugifier = slugifier;
    }

    // TODO: authz, do not allow for users
    public override void Configure()
    {
        Post("channels");
        Group<Streaming>();
        AllowAnonymous();
    }

    // TODO: cfg
    public override async Task HandleAsync(CreateChannelRequest req, CancellationToken ct)
    {
        // Don't cancel request
        var slug = _slugifier.Slugify(req.Username);
        var channel = new StreamChannel(req.Username, slug);
        channel.GenerateStreamSecretKey();

        _context.StreamChannels.Add(channel);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created new channel '{ChannelSlug}'.", slug);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create channel.");
            await Send.ErrorsAsync();
            return;
        }

        await Send.CreatedAtAsync<GetChannelBySlug>(
            routeValues: new { slug },
            responseBody: new StreamChannelResponse
            {
                Username = req.Username,
                ChannelSlug = channel.ChannelSlug,
                HlsAddress = $"http://127.0.0.1:8888/live/{slug}"
            });
    }
}
