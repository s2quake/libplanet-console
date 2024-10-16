using LibplanetConsole.Common;

namespace LibplanetConsole.Client.Guild;

internal sealed class GuildInfoProvider(Guild guildClient)
    : InfoProviderBase<IApplication>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(IApplication obj)
        => InfoUtility.EnumerateValues(guildClient.Info);
}
