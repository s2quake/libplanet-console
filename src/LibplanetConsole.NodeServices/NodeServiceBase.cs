using JSSoft.Communication;
using Libplanet.Crypto;
using Libplanet.Types.Tx;
using LibplanetConsole.NodeServices.Serializations;

namespace LibplanetConsole.NodeServices;

public abstract class NodeServiceBase(NodeBase node)
    : ServerService<INodeService, INodeCallback>, INodeService
{
    private readonly NodeBase _node = node;

    public NodeInfo Info => _node.Info;

    public bool IsRunning => _node.IsRunning;

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

    public async Task<byte[]> SendTransactionAsync(
        byte[] transaction, CancellationToken cancellationToken)
    {
        var tx = Transaction.Deserialize(transaction);
        await _node.AddTransactionAsync(tx, cancellationToken);
        return [.. tx.Id.ByteArray];
    }

    public Task<long> GetNextNonceAsync(string address, CancellationToken cancellationToken)
        => _node.GetNextNonceAsync(new Address(address), cancellationToken);

    protected override INodeService CreateServer(IPeer peer)
    {
        var service = base.CreateServer(peer);
        _node.BlockAppended += Node_BlockAppended;
        return service;
    }

    protected override void DestroyServer(IPeer peer, INodeService server)
    {
        _node.BlockAppended -= Node_BlockAppended;
        base.DestroyServer(peer, server);
    }

    private void Node_BlockAppended(object? sender, BlockEventArgs e)
    {
        var blockInfo = e.BlockInfo;
        Client.OnBlockAppended(blockInfo);
    }
}
