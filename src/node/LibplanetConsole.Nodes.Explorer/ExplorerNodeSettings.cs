using JSSoft.Commands;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Nodes.Explorer;

[ApplicationSettings]
internal sealed class ExplorerNodeSettings
{
    [CommandProperty("explorer-end-point", DefaultValue = "")]
    [CommandSummary("")]
    public string? ExplorerEndPoint { get; init; }
}
