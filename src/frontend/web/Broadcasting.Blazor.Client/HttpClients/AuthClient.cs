using System.Net.Http.Json;

using Broadcasting.Contracts.Blazor.Auth;

namespace Broadcasting.Blazor.Client.HttpClients;

public class AuthClient
{
    private readonly HttpClient _client;

    public AuthClient(HttpClient client)
    {
        _client = client;
    }

    public async Task LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync("/auth/login", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync("/auth/register", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken))!;
    }
}
