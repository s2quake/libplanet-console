using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Consoles.Explorer.Commands;

[Export(typeof(ICommand))]
[PartialCommand]
[method: ImportingConstructor]
internal sealed partial class NodeCommand(IApplication application) : CommandMethodBase
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the node. If not specified, the current node is used.")]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Start the explorer.")]
    [Category("Explorer")]
    public async Task StartExplorerAsync(
        string endPoint = "", CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(Address);
        var explorerNode = node.GetService<IExplorerNodeContent>();
        if (endPoint != string.Empty)
        {
            explorerNode.EndPoint = AppEndPoint.Parse(endPoint);
        }

        await explorerNode.StartAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Stop the explorer.")]
    [Category("Explorer")]
    public async Task StopExplorerAsync(CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(Address);
        var explorerNode = node.GetService<IExplorerNodeContent>();
        await explorerNode.StopAsync(cancellationToken);
    }
}
