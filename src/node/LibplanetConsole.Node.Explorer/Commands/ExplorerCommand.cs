using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Explorer;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Explorer.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides commands for the explorer(GraphQL).")]
[method: ImportingConstructor]
internal sealed class ExplorerCommand(IServiceProvider serviceProvider) : CommandMethodBase
{
    [CommandMethod]
    public async Task StartAsync(
        string endPoint = "", CancellationToken cancellationToken = default)
    {
        var explorer = serviceProvider.GetRequiredService<IExplorer>();
        var explorerOptions = new ExplorerOptions
        {
            EndPoint = EndPointUtility.ParseOrNext(endPoint),
        };
        await explorer.StartAsync(explorerOptions, cancellationToken);
        await Console.Out.WriteLineAsync($"http://{explorerOptions.EndPoint}/ui/playground");
    }

    [CommandMethod]
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        var explorer = serviceProvider.GetRequiredService<IExplorer>();
        await explorer.StopAsync(cancellationToken);
    }
}
