namespace Broadcasting.Contracts.Blazor.Auth;

public record LoginRequest(
    string Username,
    string Password,
    bool PersistSession);