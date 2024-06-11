using System.Collections.Immutable;
using System.Reflection;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Common;

public static class InfoUtility
{
    public static ImmutableDictionary<string, object> GetInfo(
        IEnumerable<IInfoProvider> infoProviders, object obj)
    {
        var query = from infoProvider in infoProviders
                    where infoProvider.CanSupport(obj.GetType())
                    from info in infoProvider.GetInfos(obj)
                    orderby info.Name
                    select info;

        return query.ToImmutableDictionary(info => info.Name, info => info.Value);
    }

    public static ImmutableDictionary<string, object> GetInfo(
        IServiceProvider serviceProvider, object obj)
    {
        var infoProviders = serviceProvider.GetService<IEnumerable<IInfoProvider>>();
        return GetInfo(infoProviders, obj);
    }

    public static IEnumerable<(string Name, object? Value)> EnumerateValues<T>(T info)
        where T : struct
    {
        var bindingFlags = BindingFlags.Public | BindingFlags.Instance;
        var propertyInfos = info.GetType().GetProperties(bindingFlags);
        foreach (var item in propertyInfos)
        {
            yield return (item.Name, item.GetValue(info));
        }
    }
}
