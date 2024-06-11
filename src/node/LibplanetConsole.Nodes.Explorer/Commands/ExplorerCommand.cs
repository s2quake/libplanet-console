using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Nodes.Explorer.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides commands for the explorer(GraphQL).")]
[method: ImportingConstructor]
internal sealed class ExplorerCommand(IServiceProvider serviceProvider) : CommandMethodBase
{
    [CommandMethod]
    public async Task StartAsync(
        string endPoint = "", CancellationToken cancellationToken = default)
    {
        var explorerNode = serviceProvider.GetService<IExplorerNode>();
        var explorerOptions = new ExplorerOptions
        {
            EndPoint = EndPointUtility.ParseWithFallback(endPoint),
        };
        await explorerNode.StartAsync(explorerOptions, cancellationToken);
        Console.WriteLine($"http://{explorerOptions.EndPoint}/ui/playground");
    }

    [CommandMethod]
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        var explorerContent = serviceProvider.GetService<IExplorerNode>();
        await explorerContent.StopAsync(cancellationToken);
    }
}
