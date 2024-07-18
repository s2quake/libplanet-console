using JSSoft.Commands;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Clients.Guild;

[ApplicationSettings]
internal sealed class GuildClientSettings
{
    [CommandPropertySwitch("no-guild")]
    [CommandSummary("If true, the guild client will not be started.")]
    public bool NoGuildClient { get; init; }
}
