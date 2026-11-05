using Microsoft.Extensions.Hosting;

namespace Broadcasting.Chat;

public static class Extensions
{
    public static T AddChatModule<T>(this T builder)
        where T : IHostApplicationBuilder
    {
        return builder;
    }
}
