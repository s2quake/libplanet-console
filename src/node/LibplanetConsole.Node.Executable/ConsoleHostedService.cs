using LibplanetConsole.Common;
using LibplanetConsole.Grpc;
using LibplanetConsole.Grpc.Console;
using LibplanetConsole.Node.Services;

namespace LibplanetConsole.Node.Executable;

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
        var node = serviceProvider.GetRequiredService<INode>();
        using var channel = ConsoleChannel.CreateChannel(consoleEndPoint);
        var service = new ConsoleService(channel);
        var request = new AttachNodeRequest
        {
            Address = TypeUtility.ToGrpc(node.Address),
            EndPoint = EndPointUtility.ToString(endPoint),
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
