using Broadcasting.Streaming.Data;

using FastEndpoints;

using Microsoft.EntityFrameworkCore;

namespace Broadcasting.Streaming.Endpoints.Hooks;

internal record AuthRequest(
    string User,
    string Password,
    string Token,
    string Ip,
    string Action,
    string Path,
    string Protocol,
    string Id,
    string Query,
    string UserAgent);

internal class Auth : Endpoint<AuthRequest>
{
    private readonly StreamingDbContext _context;

    public Auth(StreamingDbContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("auth");
        Group<Streaming>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(AuthRequest req, CancellationToken ct)
    {
        if (req.Action == "publish")
        {
            var secretKey = req.Path.Replace("live/", "");
            var channelExists = await _context.StreamChannels
                .AnyAsync(x => x.StreamSecretKey == secretKey, ct);

            if (channelExists)
            {
                await Send.NoContentAsync(cancellation: ct);
                return;
            }
        }

        if (req.Action == "read")
        {
            await Send.NoContentAsync(cancellation: ct);
            return;
        }

        await Send.UnauthorizedAsync(cancellation: ct);
    }
}
