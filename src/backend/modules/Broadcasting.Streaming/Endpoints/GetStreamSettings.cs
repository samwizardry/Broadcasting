using Broadcasting.Contracts.Streaming;
using Broadcasting.Streaming.Data;

using FastEndpoints;

using Microsoft.EntityFrameworkCore;

namespace Broadcasting.Streaming.Endpoints;

internal class GetStreamSettings : EndpointWithoutRequest<StreamSettingsResponse>
{
    private readonly StreamingDbContext _context;

    public GetStreamSettings(StreamingDbContext context)
    {
        _context = context;
    }

    // TODO: openid auth, get username from jwt token
    public override void Configure()
    {
        Get("channels/{username}/settings");
        Group<Streaming>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string username = Route<string>("username", true)!;
        var secKey = await _context.StreamChannels
            .AsNoTracking()
            .Where(x => x.Username == username)
            .Select(x => x.StreamSecretKey)
            .FirstOrDefaultAsync(ct);

        await Send.OkAsync(new StreamSettingsResponse(secKey), cancellation: ct);
    }
}
