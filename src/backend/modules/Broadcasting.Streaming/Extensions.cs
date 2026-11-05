using Broadcasting.ServiceDefaults;
using Broadcasting.Streaming.Data;
using Broadcasting.Streaming.Mediamtx;
using Broadcasting.Streaming.Options;

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
}
