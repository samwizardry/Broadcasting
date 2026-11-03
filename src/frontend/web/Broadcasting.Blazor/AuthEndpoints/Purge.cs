using Broadcasting.Blazor.Client.HttpClients;
using Broadcasting.Blazor.Domain.Identity;

using FastEndpoints;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Broadcasting.Blazor.AuthEndpoints;

public class Purge : EndpointWithoutRequest
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly StreamingClient _client;
    private readonly ILogger<Purge> _logger;

    public Purge(UserManager<ApplicationUser> userManager,
        StreamingClient client,
        ILogger<Purge> logger)
    {
        _userManager = userManager;
        _client = client;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/auth/purge");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var users = await _userManager.Users.ToListAsync();

        foreach (var user in users)
        {
            var result = await _userManager.DeleteAsync(user);
            var response = await _client.DeleteChannelAsync(user.UserName!);
            _logger.LogInformation("User '{Username}' deleted.", user.UserName);
        }


        await Send.NoContentAsync();
        return;
    }
}
