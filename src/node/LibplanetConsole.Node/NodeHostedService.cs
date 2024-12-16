using LibplanetConsole.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Node;

internal sealed class NodeHostedService(
    IServiceProvider serviceProvider, Node node, IApplicationOptions options)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        node.Contents = GetNodeContents(serviceProvider);
        if (options.SeedEndPoint is not null || options.IsSingleNode is true)
        {
            node.SeedEndPoint = options.SeedEndPoint;
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

    private static INodeContent[] GetNodeContents(IServiceProvider serviceProvider)
    {
        var contents = serviceProvider.GetServices<INodeContent>()
            .OrderBy(item => item.Order);
        return [.. DependencyUtility.TopologicalSort(contents, content => content.Dependencies)];
    }
}
