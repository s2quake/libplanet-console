namespace LibplanetConsole.Common.Extensions;

public static class ServiceProviderExtensions
{
    public static ILogger<T> GetLogger<T>(this IServiceProvider @this)
        => @this.GetRequiredService<ILogger<T>>();
}
