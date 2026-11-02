using FastEndpoints;

namespace Broadcasting.Streaming.Endpoints;

public class Streams : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("streams");
        Group<Streaming>();
        AllowAnonymous();
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        return base.HandleAsync(ct);
    }
}
