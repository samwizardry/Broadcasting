using Broadcasting.Blazor.Domain.Identity;
using Broadcasting.Contracts.Blazor.Auth;

using FastEndpoints;

using FluentValidation;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Broadcasting.Blazor.AuthEndpoints;

public class LoginRequestValidator : Validator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Callsign / Operator ID is required")
            .Length(3, 30)
            .WithMessage("Callsign must be 3-30 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Secret key is required")
            .Length(6, 100)
            .WithMessage("Key must be at least 6 characters");
    }
}

public class Login : Endpoint<LoginRequest>
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<Login> _logger;

    public Login(SignInManager<ApplicationUser> signInManager,
        ILogger<Login> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        var result = await _signInManager.PasswordSignInAsync(req.Username, req.Password, req.PersistSession, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in.");
            await Send.NoContentAsync();
            return;
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User account locked out.");
            AddError("This account has been locked out, please try again later.", "LockedOut");
        }
        else if (result.IsNotAllowed)
        {
            _logger.LogWarning("User is not allowed to log in.");
            AddError("Sign in not allowed.", "NotAllowed");
        }
        else
        {
            AddError("Invalid callsign or secret key.", "InvalidCredentials");
        }

        await Send.ErrorsAsync();
    }
}
