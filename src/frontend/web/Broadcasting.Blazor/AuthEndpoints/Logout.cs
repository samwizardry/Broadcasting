using Broadcasting.Blazor.Domain.Identity;

using FastEndpoints;

using Microsoft.AspNetCore.Identity;

namespace Broadcasting.Blazor.AuthEndpoints;

public class Logout : EndpointWithoutRequest
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public Logout(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public override void Configure()
    {
        Post("/auth/logout");
        AllowAnonymous();
        EnableAntiforgery();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await _signInManager.SignOutAsync();
        await Send.RedirectAsync("/", allowRemoteRedirects: false);
    }
}
