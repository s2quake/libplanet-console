using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Clients.Guild;

[Export(typeof(IInfoProvider))]
[method: ImportingConstructor]
internal sealed class GuildClientInfoProvider(GuildClient guildClient)
    : InfoProviderBase<IApplication>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(IApplication obj)
        => InfoUtility.EnumerateValues(guildClient.Info);
}
