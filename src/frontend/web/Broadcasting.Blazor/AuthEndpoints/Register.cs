using Broadcasting.Blazor.Client.HttpClients;
using Broadcasting.Blazor.Domain.Identity;
using Broadcasting.Contracts.Blazor.Auth;
using Broadcasting.Contracts.Streaming;

using FastEndpoints;

using FluentValidation;

using Microsoft.AspNetCore.Identity;

namespace Broadcasting.Blazor.AuthEndpoints;

public class RegisterRequestValidator : Validator<RegisterRequest>
{
    public RegisterRequestValidator()
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

        RuleFor(x => x.AcceptTerms)
            .Equal(true)
            .WithMessage("Protocol terms agreement is required");
    }
}

public class Register : Endpoint<RegisterRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly StreamingClient _streamingClient;
    private readonly ILogger<Register> _logger;

    public Register(UserManager<ApplicationUser> userManager,
        StreamingClient streamingClient,
        ILogger<Register> logger)
    {
        _userManager = userManager;
        _streamingClient = streamingClient;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var user = new ApplicationUser()
        {
            UserName = req.Username
        };
        var result = await _userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                AddError(error.Description, errorCode: error.Code);
            }

            await Send.ErrorsAsync(StatusCodes.Status400BadRequest);
            return;
        }

        _logger.LogTrace("User '{Username}' created a new account with password.", req.Username);

        var response = await _streamingClient.CreateChannelAsync(new CreateChannelRequest(req.Username));
        if (response.Succeeded)
        {
            _logger.LogInformation("Created new channel '{ChannelUrl}'.", response.Result!.HlsAddress);
        }
        else
        {
            _logger.LogWarning("Failed to create channel for user.");
        }

        await Send.NoContentAsync();
    }
}
