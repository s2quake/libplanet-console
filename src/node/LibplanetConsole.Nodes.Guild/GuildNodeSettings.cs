using JSSoft.Commands;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Nodes.Guild;

[ApplicationSettings]
internal sealed class GuildNodeSettings
{
    [CommandPropertySwitch("no-guild")]
    [CommandSummary("If true, the guild node will not be started.")]
    public bool NoGuildNode { get; init; }
}
