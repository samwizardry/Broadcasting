using System.Net;

using Broadcasting.Blazor.Client.HttpClients;
using Broadcasting.Blazor.Client.Services;

namespace Broadcasting.Blazor.Client;

public static class Extensions
{
    public static void AddClientServices(this IServiceCollection services, string frontendAddress, string backendAddress)
    {
        services.AddScoped<ModalService>();
        services.AddScoped<DropdownMenuService>();
        services.AddScoped<ToastService>();

        services.AddHttpClient<AuthClient>(client =>
        {
            client.BaseAddress = new Uri(frontendAddress);
        });

        services.AddHttpClient<StreamingClient>(client =>
        {
            client.BaseAddress = new Uri(backendAddress);
        });
    }

    public static void AddServerServices(this IServiceCollection services)
    {
        services.AddHttpClient<StreamingClient>(client =>
        {
            client.DefaultRequestVersion = HttpVersion.Version20;
            client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;
        });
    }
}
