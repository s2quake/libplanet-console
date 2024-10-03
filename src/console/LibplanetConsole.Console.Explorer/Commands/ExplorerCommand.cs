using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Explorer.Commands;

[Export(typeof(ICommand))]
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
        var explorer = node.GetRequiredService<IExplorer>();
        if (endPoint != string.Empty)
        {
            explorer.EndPoint = EndPointUtility.Parse(endPoint);
        }

        await explorer.StartAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Stop the explorer.")]
    [Category("Explorer")]
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(Address);
        var explorer = node.GetRequiredService<IExplorer>();
        await explorer.StopAsync(cancellationToken);
    }
}
