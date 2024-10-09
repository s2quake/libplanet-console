using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Node;

internal sealed class NodeHostedService(
    IHostApplicationLifetime applicationLifetime,
    Node node,
    ApplicationOptions options,
    ILogger<NodeHostedService> logger)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (options.SeedEndPoint is { } seedEndPoint)
        {
            applicationLifetime.ApplicationStarted.Register(async () =>
            {
                await StartNodeAsync(seedEndPoint, cancellationToken);
            });
        }

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (node.IsRunning is true)
        {
            await StopNodeAsync(cancellationToken);
        }
    }

    private async Task StartNodeAsync(EndPoint seedEndPoint, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Node auto-starting: {SeedEndPoint}", seedEndPoint);
            node.SeedEndPoint = seedEndPoint;
            await node.StartAsync(cancellationToken);
            logger.LogDebug("Node auto-started: {SeedEndPoint}", seedEndPoint);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while starting the node.");
        }
    }

    private async Task StopNodeAsync(CancellationToken cancellationToken)
    {
        try
        {
            await node.StopAsync(cancellationToken);
        }
        catch (Exception e)
        {
            await node.DisposeAsync();
            logger.LogError(e, "An error occurred while stopping the node.");
        }
    }
}
