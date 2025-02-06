using LibplanetConsole.Console.Grpc;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Grpc;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace LibplanetConsole.Node.Executable;

internal sealed class ConsoleHostedService(
    IHostApplicationLifetime applicationLifetime,
    IServer server,
    INode node,
    ConsoleConfigureOptions consoleConfigureOptions,
    ILogger<ConsoleHostedService> logger)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        applicationLifetime.ApplicationStarted.Register(AttachToConsole);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static Uri GetLocalUrl(IServer server)
    {
        var address = string.Empty;
        if (server.Features.Get<IServerAddressesFeature>() is { } addressesFeature)
        {
            address = addressesFeature.Addresses.First();
        }

        return new Uri(address);
    }

    private async void AttachToConsole()
    {
        if (consoleConfigureOptions.ConsoleUrl is { } consoleUrl)
        {
            var url = GetLocalUrl(server);
            using var channel = ConsoleChannel.CreateChannel(consoleUrl);
            var service = new ConsoleService(channel);
            var request = new AttachNodeRequest
            {
                Address = TypeUtility.ToGrpc(node.Address),
                Url = url.ToString(),
                ProcessId = Environment.ProcessId,
            };
            try
            {
                await service.AttachNodeAsync(request);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to attach the node to the console.");
            }
        }
    }
}
