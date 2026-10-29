namespace Broadcasting.Contracts.Blazor.Auth;

public record AuthError(
    string Code,
    string Description);

public class AuthResponse
{
    private static readonly AuthResponse _success = new AuthResponse() { Succeeded = true };

    public bool Succeeded { get; init; }

    public List<AuthError> Errors { get; init; } = [];

    public static AuthResponse Success => _success;

    public static AuthResponse Failure(IEnumerable<AuthError>? errors)
    {
        var respnose = new AuthResponse();

        if (errors is not null && errors.Any())
        {
            respnose.Errors.AddRange(errors);
        }

        return respnose;
    }
}
