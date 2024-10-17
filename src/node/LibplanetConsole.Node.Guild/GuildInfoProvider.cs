using LibplanetConsole.Common;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Node.Guild;

internal sealed class GuildInfoProvider(Guild guild)
    : InfoProviderBase<IHostApplicationLifetime>(nameof(Guild))
{
    protected override object? GetInfo(IHostApplicationLifetime obj) => guild.Info;
}
