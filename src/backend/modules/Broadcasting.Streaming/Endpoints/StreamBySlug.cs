using FastEndpoints;

namespace Broadcasting.Streaming.Endpoints;

public class StreamBySlug : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("streams/{slug}");
        Group<Streaming>();
        AllowAnonymous();
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        string slug = Route<string>("slug", true)!;
        return base.HandleAsync(ct);
    }
}
