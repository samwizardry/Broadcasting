using Broadcasting.Contracts.Blazor.Auth;
using Broadcasting.Contracts.Responses;

namespace Broadcasting.Blazor.Client.HttpClients;

public class AuthClient
{
    private readonly HttpClient _client;

    public AuthClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<ApiResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        return await _client.PostAsync<LoginRequest, Unit>("/auth/login", request, ct);
    }

    public async Task<ApiResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        return await _client.PostAsync<RegisterRequest, Unit>("/auth/register", request, ct);
    }

    internal async Task<ApiResponse> LogoutAsync(CancellationToken ct = default)
    {
        return await _client.PostAsync<Unit>("/auth/logout", ct);
    }
}
