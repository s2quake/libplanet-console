using JSSoft.Commands;
using LibplanetConsole.Framework;

namespace LibplanetConsole.Console.Explorer;

[ApplicationSettings]
internal sealed class ExplorerSettings
{
    [CommandPropertySwitch("explorer")]
    [CommandSummary("If set, the explorer service will also start when the node starts.")]
    public bool UseExplorer { get; init; }
}
