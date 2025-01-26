using System.Collections.Immutable;
using LibplanetConsole.Common;

namespace LibplanetConsole.Console;

internal sealed class ClientInfoProvider : InfoProviderBase<Client>
{
    public ClientInfoProvider()
        : base(nameof(Client))
    {
    }

    protected override object? GetInfo(Client obj)
    {
        var props = InfoUtility.ToDictionary(obj.Info);
        var contents = obj.GetRequiredService<IEnumerable<IClientContent>>();
        var builder = ImmutableDictionary.CreateBuilder<string, object?>();
        builder.AddRange(props);
        foreach (var content in contents)
        {
            var contentInfos = InfoUtility.GetInfo(serviceProvider: obj, obj: content);
            builder.Add(content.Name, contentInfos);
        }

        return builder.ToImmutableDictionary();
    }
}
