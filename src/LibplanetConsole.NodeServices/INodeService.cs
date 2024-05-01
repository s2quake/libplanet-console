using JSSoft.Communication;
using LibplanetConsole.NodeServices.Serializations;

namespace LibplanetConsole.NodeServices;

public interface INodeService
{
    [ServerMethod]
    Task<NodeInfo> StartAsync(NodeOptionsInfo nodeOptionsInfo, CancellationToken cancellationToken);

    [ServerMethod]
    Task StopAsync(CancellationToken cancellationToken);

    [ServerMethod]
    Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken);

    [ServerMethod]
    Task<byte[]> SendTransactionAsync(byte[] transaction, CancellationToken cancellationToken);

    [ServerMethod]
    Task<long> GetNextNonceAsync(string address, CancellationToken cancellationToken);
}
