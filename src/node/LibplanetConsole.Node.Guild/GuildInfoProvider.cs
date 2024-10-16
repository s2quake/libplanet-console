using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Guild;

internal sealed class GuildInfoProvider(GuildNode guild)
    : InfoProviderBase<IApplication>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(IApplication obj)
    {
        yield return (nameof(GuildNode), guild.Info);
    }
}
