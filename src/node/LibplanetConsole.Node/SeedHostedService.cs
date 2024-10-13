using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Node;

internal sealed class SeedHostedService(IServiceProvider serviceProvider)
    : IHostedService
{
    private readonly SeedService? _seedService = serviceProvider.GetService<SeedService>();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_seedService is not null)
        {
            await _seedService.StartAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_seedService is { IsRunning: true })
        {
            await _seedService.StopAsync(cancellationToken);
        }
    }
}
