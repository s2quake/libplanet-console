using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Node;

internal sealed class NodeHostedService(
    IServiceProvider serviceProvider, Node node, IApplicationOptions options)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        node.Contents = [.. serviceProvider.GetServices<INodeContent>()];
        node.SeedEndPoint = options.SeedEndPoint;
        if (options.ParentProcessId is 0)
        {
            await node.StartAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (node.IsRunning is true)
        {
            await node.StopAsync(cancellationToken);
        }
    }
}
