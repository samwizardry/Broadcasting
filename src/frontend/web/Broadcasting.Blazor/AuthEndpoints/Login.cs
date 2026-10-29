using Broadcasting.Blazor.Domain.Identity;
using Broadcasting.Contracts.Blazor.Auth;

using FastEndpoints;
using FastEndpoints.Security;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;

namespace Broadcasting.Blazor.AuthEndpoints;

public class Login : Endpoint<LoginRequest, AuthResponse>
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    //private readonly RoleManager<IdentityRole> _roleManager;

    public Login(SignInManager<ApplicationUser> signInManager)
        //RoleManager<IdentityRole> roleManager)
    {
        _signInManager = signInManager;
        //_roleManager = roleManager;
    }

    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    //public async Task LoginUser()
    //{
    //    if (!string.IsNullOrEmpty(Input.Passkey?.Error))
    //    {
    //        errorMessage = $"Error: {Input.Passkey.Error}";
    //        return;
    //    }

    //    SignInResult result;
    //    if (!string.IsNullOrEmpty(Input.Passkey?.CredentialJson))
    //    {
    //        // When performing passkey sign-in, don't perform form validation.
    //        result = await SignInManager.PasskeySignInAsync(Input.Passkey.CredentialJson);
    //    }
    //    else
    //    {
    //        // If doing a password sign-in, validate the form.
    //        if (!editContext.Validate())
    //        {
    //            return;
    //        }

    //        // This doesn't count login failures towards account lockout
    //        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
    //        result = await SignInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
    //    }

    //    if (result.Succeeded)
    //    {
    //        Logger.LogInformation("User logged in.");
    //        RedirectManager.RedirectTo(ReturnUrl);
    //    }
    //    else if (result.RequiresTwoFactor)
    //    {
    //        RedirectManager.RedirectTo(
    //            "Account/LoginWith2fa",
    //            new() { ["returnUrl"] = ReturnUrl, ["rememberMe"] = Input.RememberMe });
    //    }
    //    else if (result.IsLockedOut)
    //    {
    //        Logger.LogWarning("User account locked out.");
    //        RedirectManager.RedirectTo("Account/Lockout");
    //    }
    //    else
    //    {
    //        errorMessage = "Error: Invalid login attempt.";
    //    }
    //}

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        // Clear the existing external cookie to ensure a clean login process
        //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);



        var user = await _signInManager.UserManager.FindByNameAsync(req.Username);

        if (user is null)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        if (!await _signInManager.CanSignInAsync(user))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, req.Password, true);

        if (!signInResult.Succeeded)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var principal = await _signInManager.CreateUserPrincipalAsync(user);
        //var userRoles = await _signInManager.UserManager.GetRolesAsync(user);

        await CookieAuth.SignInAsync(u =>
        {
            foreach (var claim in principal.Claims)
            {
                u.Claims.Add(claim);
            }

            //foreach (var role in userRoles)
            //{
            //    u.Roles.Add(role);
            //}

            u.Roles.Add("TestRole");
            u.Permissions.Add("TestPermission");
        },
        p =>
        {
            p.AllowRefresh = true;
            p.IsPersistent = req.PersistSession;
        });
    }
}
