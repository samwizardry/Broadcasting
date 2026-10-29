using Broadcasting.Blazor.Client.HttpClients;

namespace Broadcasting.Blazor.Client.Services;

public static class ServiceExtensions
{
    public static void AddClientServices(this IServiceCollection services, string baseAddress)
    {
        services.AddScoped<ModalService>();

        services.AddHttpClient<AuthClient>(httpClient =>
        {
            httpClient.BaseAddress = new Uri(baseAddress);
        });
    }
}
