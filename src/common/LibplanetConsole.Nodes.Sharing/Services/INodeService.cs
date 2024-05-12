using LibplanetConsole.Nodes.Serializations;

namespace LibplanetConsole.Nodes.Services;

public interface INodeService
{
    Task<NodeInfo> StartAsync(NodeOptionsInfo nodeOptionsInfo, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken);

    Task<byte[]> SendTransactionAsync(byte[] transaction, CancellationToken cancellationToken);

    Task<long> GetNextNonceAsync(string address, CancellationToken cancellationToken);
}
