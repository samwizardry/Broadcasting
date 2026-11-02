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

    public async Task<ApiResponse<ProblemDetails>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        return await _client.PostAsync<LoginRequest, Unit>("/auth/login", request, ct);
    }

    public async Task<ApiResponse<ProblemDetails>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        return await _client.PostAsync<RegisterRequest, Unit>("/auth/register", request, ct);
    }

    public async Task<ApiResponse<ProblemDetails>> LogoutAsync(CancellationToken ct = default)
    {
        return await _client.PostAsync<Unit, Unit>("/auth/logout", default, ct);
    }
}
