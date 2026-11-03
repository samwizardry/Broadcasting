using Broadcasting.Streaming.Data;
using Broadcasting.Streaming.Http;

using FastEndpoints;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Broadcasting.Streaming.Endpoints;

internal class DeleteChannel : EndpointWithoutRequest
{
    private readonly ILogger<DeleteChannel> _logger;
    private readonly StreamingDbContext _context;
    private readonly MediamtxClient _client;

    public DeleteChannel(ILogger<DeleteChannel> logger,
        StreamingDbContext context,
        MediamtxClient client)
    {
        _logger = logger;
        _context = context;
        _client = client;
    }

    public override void Configure()
    {
        Delete("channels/{username}");
        Group<Streaming>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Don't cancel request
        string username = Route<string>("username", true)!;

        var channel = await _context.StreamChannels.FirstOrDefaultAsync(x => x.Username == username);
        if (channel is null)
        {
            await Send.NotFoundAsync();
            return;
        }

        _context.StreamChannels.Remove(channel);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Channel '{ChannelSlug}' deleted.", channel.ChannelSlug);

        var response = await _client.DeletePathAsync(channel.ChannelSlug);
        if (response.Succeeded)
        {
            _logger.LogInformation("Channel path '{Path}' deleted.", channel.ChannelSlug);
        }
        else
        {
            _logger.LogError("Failed to delete channel path. {Error}", response.Error.Error);
            await Send.ErrorsAsync();
            return;
        }

        await Send.NoContentAsync(cancellation: ct);
    }
}
