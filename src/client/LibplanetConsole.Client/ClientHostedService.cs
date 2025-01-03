using LibplanetConsole.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Client;

internal sealed class ClientHostedService(
    IServiceProvider serviceProvider, Client client, IApplicationOptions options)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        client.Contents = GetClientContents(serviceProvider);
        if (options.NodeEndPoint is not null)
        {
            client.NodeEndPoint = options.NodeEndPoint;
            await client.StartAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (client.IsRunning is true)
        {
            await client.StopAsync(cancellationToken);
        }
    }

    private static IClientContent[] GetClientContents(IServiceProvider serviceProvider)
    {
        var contents = serviceProvider.GetServices<IClientContent>()
            .OrderBy(item => item.Order);
        return [.. DependencyUtility.TopologicalSort(contents, content => content.Dependencies)];
    }
}
