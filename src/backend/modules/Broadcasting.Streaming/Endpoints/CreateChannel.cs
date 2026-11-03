using System.Net.Http.Json;

using Broadcasting.Contracts.Streaming;
using Broadcasting.SharedKernel.Abstractions;
using Broadcasting.Streaming.Data;
using Broadcasting.Streaming.Domain;
using Broadcasting.Streaming.Http;

using FastEndpoints;

using Microsoft.Extensions.Logging;

namespace Broadcasting.Streaming.Endpoints;

internal class CreateChannel : Endpoint<CreateChannelRequest, StreamChannelResponse>
{
    private readonly ILogger<CreateChannel> _logger;
    private readonly StreamingDbContext _context;
    private readonly ISlugifier _slugifier;
    private readonly MediamtxClient _client;

    public CreateChannel(ILogger<CreateChannel> logger,
        StreamingDbContext context,
        ISlugifier slugifier,
        MediamtxClient client)
    {
        _logger = logger;
        _context = context;
        _slugifier = slugifier;
        _client = client;
    }

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

        var response = await _client.AddPathAsync(new($"rtsp://127.0.0.1:8554/live/{channel.StreamSecretKey}"), slug);
        if (response.Succeeded)
        {
            _logger.LogInformation("Added channel path");
        }
        else
        {
            _logger.LogError("Failed to add channel path. {Error}", response.Error.Error);

            await Send.ErrorsAsync();
            return;
        }

        await Send.CreatedAtAsync<GetChannelBySlug>(
            routeValues: new { slug },
            responseBody: new StreamChannelResponse
            {
                Username = req.Username,
                RtspUrl = $"rtsp://127.0.0.1:8554/{slug}"
            });
    }
}
