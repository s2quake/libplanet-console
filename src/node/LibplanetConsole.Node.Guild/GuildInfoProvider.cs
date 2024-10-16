using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Guild;

internal sealed class GuildInfoProvider(Guild guild)
    : InfoProviderBase<IHostApplicationLifetime>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(IApplication obj)
    {
        yield return (nameof(Guild), guild.Info);
    }
}
