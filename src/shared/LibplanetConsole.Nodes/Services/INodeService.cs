namespace LibplanetConsole.Nodes.Services;

public interface INodeService
{
    Task<NodeInfo> StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken);
}
