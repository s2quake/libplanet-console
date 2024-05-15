using System.Diagnostics.CodeAnalysis;

namespace LibplanetConsole.Common.Extensions;

public static class IServiceProviderExtensions
{
    public static T GetService<T>(this IServiceProvider @this)
        where T : notnull
    {
        if (@this.GetService(typeof(T)) is T service)
        {
            return service;
        }

        throw new InvalidOperationException($"Service '{typeof(T)}' is not found.");
    }

    public static bool TryGetService<T>(
        this IServiceProvider @this, [MaybeNullWhen(false)] out T service)
        where T : class
    {
        if (@this.GetService(typeof(T)) is T value)
        {
            service = value;
            return true;
        }

        service = default;
        return false;
    }
}
