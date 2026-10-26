namespace Broadcasting.Blazor.Client.Services;

public static class ServiceExtensions
{
    public static void AddClientServices(this IServiceCollection services)
    {
        services.AddScoped<ModalService>();
    }
}
