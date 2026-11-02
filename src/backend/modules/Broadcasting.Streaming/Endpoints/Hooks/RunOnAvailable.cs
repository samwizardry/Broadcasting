using FastEndpoints;

using Microsoft.Extensions.Logging;

namespace Broadcasting.Streaming.Endpoints.Hooks;

internal class RunOnAvailable : EndpointWithoutRequest
{
    private readonly ILogger<RunOnAvailable> _logger;

    public RunOnAvailable(ILogger<RunOnAvailable> logger)
    {
        _logger = logger;
    }

    public override void Configure()
    {
        Get("run-on-available");
        Group<Streaming>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string path = Query<string>("path") ?? "";
        string sourceType = Query<string>("source_type") ?? "";

        _logger.LogInformation("Stream '{Path}' is now availble.", path);

        await Send.NoContentAsync(cancellation: ct);
    }
}
