using LibplanetConsole.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Console;

internal sealed class ConsoleHostedService(
    IServiceProvider serviceProvider,
    SeedService seedService,
    ConsoleHost console,
    IHostApplicationLifetime applicationLifetime)
    : IHostedService
{
    private CancellationTokenSource? _cancellationTokenSource;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        console.Contents = GetConsoleContents(serviceProvider);
        applicationLifetime.ApplicationStarted.Register(
            async () => await console.InitializeAsync());
        await seedService.StartAsync(cancellationToken);
        await console.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        await console.StopAsync(cancellationToken);
        await seedService.StopAsync(cancellationToken);
    }

    private static IConsoleContent[] GetConsoleContents(IServiceProvider serviceProvider)
    {
        var contents = serviceProvider.GetServices<IConsoleContent>();
        return [.. DependencyUtility.TopologicalSort(contents, content => content.Dependencies)];
    }
}
