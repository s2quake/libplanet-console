using Grpc.Core;
using LibplanetConsole.Common;
using LibplanetConsole.Hub.Grpc;
using LibplanetConsole.Hub.Services;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Node;

internal sealed class NodeHostedService(
    IServiceProvider serviceProvider, Node node, IApplicationOptions options)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        node.Contents = GetNodeContents(serviceProvider);
        if (options.HubUrl is { } hubUrl)
        {
            using var channel = HubChannel.CreateChannel(hubUrl);
            var service = new HubService(channel);
            var request = new GetServiceRequest
            {
                ServiceName = "libplanet.console.seed.v1",
            };
            var callOptions = new CallOptions(cancellationToken: cancellationToken);
            var response = await service.GetServiceAsync(request, callOptions);
            node.SeedUrl = new(response.Url);
            await node.StartAsync(cancellationToken);
        }
        else if (options.IsSingleNode is true)
        {
            node.SeedUrl = null;
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
