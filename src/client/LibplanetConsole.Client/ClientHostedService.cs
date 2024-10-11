using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Client;

internal sealed class ClientHostedService(
    IHostApplicationLifetime applicationLifetime,
    ApplicationOptions options,
    Client client,
    ILogger<ClientHostedService> logger)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        applicationLifetime.ApplicationStarted.Register(async () =>
        {
            if (options.NodeEndPoint is not null)
            {
                logger.LogDebug("Client auto-starting");
                await client.StartAsync(cancellationToken);
                logger.LogDebug("Client auto-started");
            }
        });
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (client.IsRunning is true)
        {
            await client.StopAsync(cancellationToken);
        }
    }
}
