using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Node;

internal sealed class NodeHostedService(
    IHostApplicationLifetime applicationLifetime,
    SeedService seedService,
    Node node,
    ApplicationOptions options,
    ILogger<NodeHostedService> logger)
    : IHostedService
{
    private CancellationTokenSource? _cancellationTokenSource;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        applicationLifetime.ApplicationStarted.Register(async () =>
        {
            _cancellationTokenSource = new();
            try
            {
                await ExecuteAsync(_cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occurred while starting the node.");
                applicationLifetime.StopApplication();
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

        await seedService.StopAsync(cancellationToken);
    }

    private async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        if (CompareEndPoint(options.SeedEndPoint, options.EndPoint) is true)
        {
            await seedService.StartAsync(cancellationToken);
        }

        if (options.SeedEndPoint is { } seedEndPoint)
        {
            node.SeedEndPoint = seedEndPoint;
            await node.StartAsync(cancellationToken);
        }
    }
}
