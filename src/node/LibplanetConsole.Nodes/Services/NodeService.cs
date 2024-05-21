using System.ComponentModel.Composition;
using Libplanet.Crypto;
using Libplanet.Types.Tx;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Nodes.Serializations;

namespace LibplanetConsole.Nodes.Services;

[Export(typeof(ILocalService))]
[Export(typeof(INodeService))]
internal sealed class NodeService : LocalService<INodeService, INodeCallback>, INodeService
{
    private readonly Node _node;

    [ImportingConstructor]
    public NodeService(Node node)
    {
        _node = node;
        _node.Started += (s, e) => Callback.OnStarted(_node.Info);
        _node.Stopped += (s, e) => Callback.OnStopped();
        _node.BlockAppended += Node_BlockAppended;
    }

    public async Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return _node.Info;
    }

    public async Task<NodeInfo> StartAsync(
        NodeOptionsInfo nodeOptionsInfo, CancellationToken cancellationToken)
    {
        await _node.StartAsync(nodeOptionsInfo, cancellationToken);
        return _node.Info;
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => _node.StopAsync(cancellationToken);

    public async Task<TxId> SendTransactionAsync(
        byte[] transaction, CancellationToken cancellationToken)
    {
        var tx = Transaction.Deserialize(transaction);
        await _node.AddTransactionAsync(tx, cancellationToken);
        return tx.Id;
    }

    public Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken)
    {
        return Task.Run(() => _node.GetNextNonce(address), cancellationToken);
    }

    private void Node_BlockAppended(object? sender, BlockEventArgs e)
    {
        var blockInfo = e.BlockInfo;
        Callback.OnBlockAppended(blockInfo);
    }
}
