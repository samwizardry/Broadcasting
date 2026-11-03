using Broadcasting.Blazor.Client.HttpClients;

namespace Broadcasting.Blazor.Client.Services;

public static class ServiceExtensions
{
    public static void AddClientServices(this IServiceCollection services, string baseAddress)
    {
        services.AddScoped<ModalService>();
        services.AddScoped<DropdownMenuService>();
        services.AddScoped<ToastService>();

        services.AddHttpClient<AuthClient>(httpClient =>
        {
            httpClient.BaseAddress = new Uri(baseAddress);
        });

        services.AddHttpClient<StreamingClient>(httpClient =>
        {
            httpClient.BaseAddress = new Uri(baseAddress);
        });
    }
}
