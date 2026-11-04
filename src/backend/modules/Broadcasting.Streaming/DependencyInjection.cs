using Broadcasting.ServiceDefaults;
using Broadcasting.Streaming.Data;
using Broadcasting.Streaming.Http;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Broadcasting.Streaming;

public static class DependencyInjection
{
    public static T AddStreamingModule<T>(this T builder)
        where T : IHostApplicationBuilder
    {
        var connectionString = builder.Configuration.GetConnectionString("Streaming") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<StreamingDbContext>(options =>
            options.UseNpgsql(connectionString));

        builder.Services.AddHttpClient<MediamtxClient>(options =>
        {
            // TODO: cfg
            options.BaseAddress = new Uri($"http://{KnownServices.MediaMTX}");
        });

        return builder;
    }
}
