using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes.Guild;

[Export(typeof(IInfoProvider))]
[method: ImportingConstructor]
internal sealed class GuildNodeInfoProvider(GuildNode guildNode)
    : InfoProviderBase<IApplication>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(IApplication obj)
    {
        yield return (nameof(GuildNode), guildNode.Info);
    }
}
