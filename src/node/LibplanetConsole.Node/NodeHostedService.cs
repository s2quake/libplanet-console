using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Node;

internal sealed class NodeHostedService(
    IHostApplicationLifetime applicationLifetime, Node node, ILogger<NodeHostedService> logger)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        applicationLifetime.ApplicationStarted.Register(async () =>
        {
            logger.LogInformation("Application started.");
            if (node.SeedEndPoint is { } seedEndPoint)
            {
                logger.LogDebug("Node auto-starting");
                node.SeedEndPoint = seedEndPoint;
                await node.StartAsync(cancellationToken);
                logger.LogDebug("Node auto-started");
            }
        });
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (node.IsRunning is true)
        {
            await node.StopAsync(cancellationToken);
        }
    }
}
