using Broadcasting.ServiceDefaults;
using Broadcasting.Streaming.Data;
using Broadcasting.Streaming.Mediamtx;
using Broadcasting.Streaming.Options;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Broadcasting.Streaming;

public static class Extensions
{
    public static T AddStreamingModule<T>(this T builder)
        where T : IHostApplicationBuilder
    {
        var connectionString = builder.Configuration.GetConnectionString("Streaming") ?? throw new InvalidOperationException("Connection string 'Streaming' not found.");

        builder.Services.AddDbContext<StreamingDbContext>(options =>
            options.UseNpgsql(connectionString));

        builder.Services.AddHttpClient<MediamtxClient>(options =>
        {
            options.BaseAddress = new Uri($"http://{KnownServices.MediaMTX}");
        });

        builder.Services.AddOptions<StreamingOptions>()
            .BindConfiguration(StreamingOptions.Key)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return builder;
    }

    public static async Task<WebApplication> MigrateStreamingAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<StreamingDbContext>();
        await context.Database.MigrateAsync();
        return app;
    }
}
