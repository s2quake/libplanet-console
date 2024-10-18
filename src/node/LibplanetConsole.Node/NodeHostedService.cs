using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Node;

internal sealed class NodeHostedService(
    IServiceProvider serviceProvider, Node node, ApplicationOptions options)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        node.Contents = [.. serviceProvider.GetServices<INodeContent>()];
        if (options.SeedEndPoint is { } seedEndPoint)
        {
            node.SeedEndPoint = seedEndPoint;
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
