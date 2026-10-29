namespace Broadcasting.Contracts.Blazor.Auth;

public record RegisterRequest(
    string Username,
    string Password,
    bool AcceptTerms);
