using Broadcasting.Blazor.Data;
using Broadcasting.Contracts.Blazor.Auth;

using FastEndpoints;
using FastEndpoints.Security;

using Microsoft.AspNetCore.Identity;

namespace Broadcasting.Blazor.AuthEndpoints;

public class Login : Endpoint<LoginRequest>
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

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _signInManager.UserManager.FindByNameAsync(req.Username);

        if (user is null)
        {
            await HttpContext.Response.SendUnauthorizedAsync(ct);
            return;
        }

        if (!await _signInManager.CanSignInAsync(user))
        {
            await HttpContext.Response.SendUnauthorizedAsync(ct);
            return;
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, req.Password, true);

        if (!signInResult.Succeeded)
        {
            await HttpContext.Response.SendUnauthorizedAsync(ct);
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
