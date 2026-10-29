using Broadcasting.Blazor.Domain.Identity;
using Broadcasting.Contracts.Blazor.Auth;

using FastEndpoints;
using FastEndpoints.Security;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Broadcasting.Blazor.AuthEndpoints;

public class Register : Endpoint<RegisterRequest, AuthResponse>
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly ILogger<Register> _logger;

    public Register(SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        ILogger<Register> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var user = new ApplicationUser();
        await _userStore.SetUserNameAsync(user, req.Username, CancellationToken.None);

        var result = await _userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            await Send.OkAsync(
                AuthResponse.Failure(result.Errors.Select(x => new AuthError(x.Code, x.Description))),
                CancellationToken.None);

            return;
        }

        _logger.LogTrace("User '{Username}' created a new account with password '{Password}'.", req.Username, req.Password);

        await _signInManager.SignInAsync(user, true);
        await Send.OkAsync(AuthResponse.Success, CancellationToken.None);
    }
}
