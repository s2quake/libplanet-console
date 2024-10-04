using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Common.Extensions;

public static class ServiceProviderExtensions
{
    public static ILogger<T> GetLogger<T>(this IServiceProvider @this)
        => @this.GetRequiredService<ILogger<T>>();
}
