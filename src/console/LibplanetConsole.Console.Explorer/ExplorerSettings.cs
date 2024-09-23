using JSSoft.Commands;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles.Explorer;

[ApplicationSettings]
internal sealed class ExplorerSettings
{
    [CommandPropertySwitch("explorer")]
    [CommandSummary("If set, the explorer service will also start when the node starts.")]
    public bool UseExplorer { get; init; }
}
