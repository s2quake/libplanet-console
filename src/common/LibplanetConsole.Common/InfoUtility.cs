using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Common;

public static class InfoUtility
{
    public static ImmutableDictionary<string, object?> GetInfo(
        IEnumerable<IInfoProvider> infoProviders, object obj)
    {
        var query = from infoProvider in infoProviders
                    where infoProvider.CanSupport(obj.GetType())
                    let name = infoProvider.Name
                    let value = infoProvider.GetInfo(obj)
                    orderby name
                    select new KeyValuePair<string, object?>(name, value);
        var builder = ImmutableDictionary.CreateBuilder<string, object?>();
        foreach (var info in query)
        {
            builder.Add(info.Key, info.Value);
        }

        return builder.ToImmutableDictionary();
    }

    public static ImmutableDictionary<string, object?> GetInfo(
        IServiceProvider serviceProvider, object obj)
    {
        var infoProviders = serviceProvider.GetServices<IInfoProvider>();
        return GetInfo(infoProviders, obj);
    }

    public static ImmutableDictionary<string, object?> ToDictionary<T>(T info)
        where T : struct
    {
        var bindingFlags = BindingFlags.Public | BindingFlags.Instance;
        var propertyInfos = info.GetType().GetProperties(bindingFlags);
        var builder = ImmutableDictionary.CreateBuilder<string, object?>();
        foreach (var item in propertyInfos)
        {
            builder.Add(item.Name, item.GetValue(info));
        }

        return builder.ToImmutableDictionary();
    }
}
