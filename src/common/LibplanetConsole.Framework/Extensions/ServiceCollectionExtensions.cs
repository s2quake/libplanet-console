using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Framework.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingletonWithInterface<TInterface, TClass>(
        this IServiceCollection @this)
        where TInterface : class
        where TClass : class, TInterface
    {
        @this.AddSingleton<TClass>();
        @this.AddSingleton<TInterface>(s => s.GetRequiredService<TClass>());
        return @this;
    }

    public static IServiceCollection AddSingletonWithInterfaces<TInterface1, TInterface2, TClass>(
        this IServiceCollection @this)
        where TInterface1 : class
        where TInterface2 : class
        where TClass : class, TInterface1, TInterface2
    {
        @this.AddSingleton<TClass>();
        @this.AddSingleton<TInterface1>(s => s.GetRequiredService<TClass>());
        @this.AddSingleton<TInterface2>(s => s.GetRequiredService<TClass>());
        return @this;
    }
}
