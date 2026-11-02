using FastEndpoints;

using Microsoft.Extensions.Logging;

namespace Broadcasting.Streaming.Endpoints.Hooks;

internal class RunOnUnavailable : EndpointWithoutRequest
{
    private readonly ILogger<RunOnUnavailable> _logger;

    public RunOnUnavailable(ILogger<RunOnUnavailable> logger)
    {
        _logger = logger;
    }

    public override void Configure()
    {
        Get("run-on-unavailable");
        Group<Streaming>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string path = Query<string>("path") ?? "";
        string sourceType = Query<string>("source_type") ?? "";

        _logger.LogInformation("Stream '{Path}' is no longer available.", path);

        await Send.NoContentAsync(cancellation: ct);
    }
}
