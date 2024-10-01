using System.Collections.Immutable;
using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console;

[Export(typeof(IInfoProvider))]
internal sealed class NodeInfoProvider : InfoProviderBase<Node>
{
    public NodeInfoProvider()
        : base(nameof(Node))
    {
    }

    protected override object? GetInfo(Node obj)
    {
        var props = InfoUtility.ToDictionary(obj.Info);
        var contents = obj.GetRequiredService<IEnumerable<INodeContent>>();
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
