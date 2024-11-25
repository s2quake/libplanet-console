using LibplanetConsole.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Console;

internal sealed class ConsoleHostedService(
    SeedService seedService,
    NodeCollection nodes,
    ClientCollection clients,
    IHostApplicationLifetime applicationLifetime,
    ApplicationInfoProvider applicationInfoProvider,
    ILogger<ConsoleHostedService> logger)
    : IHostedService
{
    private CancellationTokenSource? _cancellationTokenSource;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        applicationLifetime.ApplicationStarted.Register(Initialize);
        await seedService.StartAsync(cancellationToken);
        await nodes.StartAsync(cancellationToken);
        await clients.StartAsync(cancellationToken);

        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        await clients.StopAsync(cancellationToken);
        await nodes.StopAsync(cancellationToken);
        await seedService.StopAsync(cancellationToken);
    }

    private async void Initialize()
    {
        _cancellationTokenSource = new();
        try
        {
            logger.LogDebug(JsonUtility.Serialize(applicationInfoProvider.Info));
            await nodes.InitializeAsync(_cancellationTokenSource.Token);
            await clients.InitializeAsync(_cancellationTokenSource.Token);
        }
        catch (OperationCanceledException e)
        {
            logger.LogDebug(e, "The console was canceled.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while starting the console.");
        }
    }
}
