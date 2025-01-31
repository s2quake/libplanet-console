using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Console.Seed;

internal sealed class SeedHostedService(SeedService seedService)
    : IHostedService
{
    private CancellationTokenSource? _cancellationTokenSource;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await seedService.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        await seedService.StopAsync(cancellationToken);
    }
}
