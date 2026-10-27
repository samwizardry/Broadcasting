using System.Net.Http.Json;

using Broadcasting.Contracts.Blazor.Auth;

namespace Broadcasting.Blazor.Client.HttpClients;

public class AuthHttpClient : HttpClient
{
    private readonly HttpClient _client;

    public AuthHttpClient(HttpClient client)
    {
        _client = client;
    }

    public async Task LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _client.PostAsJsonAsync("/auth/login", request, cancellationToken);
        result.EnsureSuccessStatusCode();
    }
}
