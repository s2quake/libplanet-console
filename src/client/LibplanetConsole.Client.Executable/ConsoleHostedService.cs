using LibplanetConsole.Client.Services;
using LibplanetConsole.Common;
using LibplanetConsole.Grpc;
using LibplanetConsole.Grpc.Console;

namespace LibplanetConsole.Client.Executable;

internal sealed class ConsoleHostedService(
    IServiceProvider serviceProvider, int port, EndPoint consoleEndPoint)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var applicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();
        applicationLifetime.ApplicationStarted.Register(AttachToConsole);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async void AttachToConsole()
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ConsoleHostedService>>();
        var endPoint = EndPointUtility.GetLocalHost(port);
        var node = serviceProvider.GetRequiredService<IClient>();
        using var channel = ConsoleChannel.CreateChannel(consoleEndPoint);
        var service = new ConsoleService(channel);
        var request = new AttachClientRequest
        {
            Address = TypeUtility.ToGrpc(node.Address),
            EndPoint = EndPointUtility.ToString(endPoint),
            ProcessId = Environment.ProcessId,
        };
        try
        {
            await service.AttachClientAsync(request);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to attach the node to the console.");
        }
    }
}
