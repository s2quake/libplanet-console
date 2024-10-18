using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Client;

internal sealed class ClientHostedService(
    IServiceProvider serviceProvider, Client client, ApplicationOptions options)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        client.Contents = [.. serviceProvider.GetServices<IClientContent>()];
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
}
