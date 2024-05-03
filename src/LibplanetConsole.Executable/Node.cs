using System.Net;
using JSSoft.Communication;
using Libplanet.Action;
using Libplanet.Crypto;
using Libplanet.Types.Blocks;
using Libplanet.Types.Tx;
using LibplanetConsole.Common;
using LibplanetConsole.NodeServices;
using LibplanetConsole.NodeServices.Serializations;

namespace LibplanetConsole.Executable;

internal sealed class Node
    : INodeCallback, IAsyncDisposable, IIdentifier, INode
{
    private readonly PrivateKey _privateKey;
    private readonly ClientContext _clientContext;
    private readonly ClientService<INodeService, INodeCallback> _nodeService;
    private DnsEndPoint? _swarmEndPoint;
    private DnsEndPoint? _consensusEndPoint;
    private Guid _closeToken;
    private NodeInfo _nodeInfo = new();

    public Node(PrivateKey privateKey, EndPoint endPoint)
    {
        _privateKey = privateKey;
        _nodeService = new(this);
        _clientContext = new ClientContext(
            _nodeService)
        {
            EndPoint = endPoint,
        };
        _clientContext.Disconnected += ClientContext_Disconnected;
        _clientContext.Faulted += ClientContext_Faulted;
    }

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public event EventHandler? Disposed;

    public DnsEndPoint SwarmEndPoint
        => _swarmEndPoint ?? throw new InvalidOperationException("Peer is not set.");

    public DnsEndPoint ConsensusEndPoint
        => _consensusEndPoint ?? throw new InvalidOperationException("ConsensusPeer is not set.");

    public PrivateKey PrivateKey => _privateKey;

    public PublicKey PublicKey => _privateKey.PublicKey;

    public Address Address => _privateKey.Address;

    public bool IsRunning { get; private set; }

    public string Identifier => Address.ToString()[0..8];

    public NodeOptions NodeOptions { get; private set; } = NodeOptions.Default;

    public EndPoint EndPoint => _clientContext.EndPoint;

    public NodeInfo Info => _nodeInfo;

    PrivateKey IIdentifier.PrivateKey => _privateKey;

    public async ValueTask DisposeAsync()
    {
        await _clientContext.CloseAsync(_closeToken, cancellationToken: default);
    }

    public override string ToString()
    {
        return $"{Address.ToString()[0..8]}: {EndPointUtility.ToString(EndPoint)}";
    }

    public async Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        _nodeInfo = await _nodeService.Server.GetInfoAsync(cancellationToken);
        return _nodeInfo;
    }

    public async Task StartAsync(NodeOptions nodeOptions, CancellationToken cancellationToken)
    {
        if (IsRunning == true)
        {
            throw new InvalidOperationException("Node is already running.");
        }

        _closeToken = await _clientContext.OpenAsync(cancellationToken);
        _nodeInfo = await _nodeService.Server.StartAsync(nodeOptions, cancellationToken);
        _swarmEndPoint = DnsEndPointUtility.GetEndPoint(_nodeInfo.SwarmEndPoint);
        _consensusEndPoint = DnsEndPointUtility.GetEndPoint(_nodeInfo.ConsensusEndPoint);
        NodeOptions = nodeOptions;
        IsRunning = true;
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (IsRunning != true)
        {
            throw new InvalidOperationException("Node is not running.");
        }

        NodeOptions = NodeOptions.Default;
        IsRunning = false;
        await _nodeService.Server.StopAsync(cancellationToken);
        await _clientContext.CloseAsync(_closeToken, cancellationToken);
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async Task<TxId> AddTransactionAsync(
        IAction[] actions, CancellationToken cancellationToken)
    {
        var address = Address.ToString();
        var values = actions.Select(item => item.PlainValue).ToArray();
        var nonce = await _nodeService.Server.GetNextNonceAsync(address, cancellationToken);
        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: _privateKey,
            genesisHash: BlockHash.FromString(_nodeInfo.GenesisHash),
            actions: new TxActionList(values)
        );

        var bytes = await _nodeService.Server.SendTransactionAsync(
            transaction: transaction.Serialize(),
            cancellationToken: cancellationToken);

        return new TxId(bytes);
    }

    public object? GetService(Type serviceType)
    {
        return null;
    }

    void INodeCallback.OnBlockAppended(BlockInfo blockInfo)
    {
        BlockAppended?.Invoke(this, new BlockEventArgs(blockInfo));
    }

    private void ClientContext_Disconnected(object? sender, EventArgs e)
    {
        Disposed?.Invoke(this, EventArgs.Empty);
    }

    private void ClientContext_Faulted(object? sender, EventArgs e)
    {
        Disposed?.Invoke(this, EventArgs.Empty);
    }
}
