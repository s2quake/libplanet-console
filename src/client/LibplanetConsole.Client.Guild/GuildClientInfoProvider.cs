using LibplanetConsole.Common;

namespace LibplanetConsole.Client.Guild;

[Export(typeof(IInfoProvider))]
internal sealed class GuildClientInfoProvider(GuildClient guildClient)
    : InfoProviderBase<IApplication>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(IApplication obj)
        => InfoUtility.EnumerateValues(guildClient.Info);
}
