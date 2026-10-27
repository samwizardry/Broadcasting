using Broadcasting.Identity.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Broadcasting.Identity;

public static class DependencyInjection
{
    public static T AddIdentityModule<T>(T builder)
        where T : IHostApplicationBuilder
    {
        var connectionString = builder.Configuration.GetConnectionString("Identity") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString));



        return builder;
    }
}
