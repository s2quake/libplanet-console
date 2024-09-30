using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Explorer.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
internal sealed partial class ExplorerCommand(
    NodeCommand nodeCommand, IApplication application)
    : CommandMethodBase(nodeCommand)
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the node. If not specified, the current node is used.")]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Start the explorer.")]
    [Category("Explorer")]
    public async Task StartAsync(
        string endPoint = "", CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(Address);
        var explorerNode = node.GetRequiredService<IExplorerNodeContent>();
        if (endPoint != string.Empty)
        {
            explorerNode.EndPoint = EndPointUtility.Parse(endPoint);
        }

        await explorerNode.StartAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Stop the explorer.")]
    [Category("Explorer")]
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(Address);
        var explorerNode = node.GetRequiredService<IExplorerNodeContent>();
        await explorerNode.StopAsync(cancellationToken);
    }
}
