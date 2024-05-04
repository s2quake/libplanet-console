using System.Collections;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Net;
using JSSoft.Communication;
using JSSoft.Communication.Extensions;
using Libplanet.Action;
using Libplanet.Crypto;
using Libplanet.Types.Blocks;
using Libplanet.Types.Tx;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.NodeServices;
using LibplanetConsole.NodeServices.Serializations;

namespace LibplanetConsole.Executable;

internal sealed class Node
    : INodeCallback, IAsyncDisposable, IAddressable, INode
{
    private readonly CompositionContainer _container;
    private readonly PrivateKey _privateKey;
    private readonly RemoteContext _remoteContext;
    private readonly RemoteService<INodeService, INodeCallback> _remoteService;
    private readonly INodeContent[] _contents;
    private DnsEndPoint? _blocksyncEndPoint;
    private DnsEndPoint? _consensusEndPoint;
    private Guid _closeToken;
    private NodeInfo _nodeInfo = new();
    private bool _isDisposed;

    public Node(CompositionContainer container, PrivateKey privateKey, EndPoint endPoint)
    {
        _container = container;
        _privateKey = privateKey;
        _remoteService = new(this);
        _remoteContext = new RemoteContext(
            _remoteService)
        {
            EndPoint = endPoint,
        };
        _container.ComposeExportedValue<INode>(this);
        _contents = [.. _container.GetExportedValues<INodeContent>()];
        _remoteContext.Disconnected += RemoteContext_Disconnected;
        _remoteContext.Faulted += RemoteContext_Faulted;
    }

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public event EventHandler? Disposed;

    public DnsEndPoint SwarmEndPoint
        => _blocksyncEndPoint ?? throw new InvalidOperationException("Peer is not set.");

    public DnsEndPoint ConsensusEndPoint
        => _consensusEndPoint ?? throw new InvalidOperationException("ConsensusPeer is not set.");

    public PrivateKey PrivateKey => _privateKey;

    public PublicKey PublicKey => _privateKey.PublicKey;

    public Address Address => _privateKey.Address;

    public bool IsRunning { get; private set; }

    public NodeOptions NodeOptions { get; private set; } = NodeOptions.Default;

    public EndPoint EndPoint => _remoteContext.EndPoint;

    public NodeInfo Info => _nodeInfo;

    public object? GetService(Type serviceType)
    {
        if (serviceType == typeof(IServiceProvider))
        {
            return this;
        }

        if (typeof(IEnumerable).IsAssignableFrom(serviceType) &&
            serviceType.GenericTypeArguments.Length == 1)
        {
            var itemType = serviceType.GenericTypeArguments.First();
            var items = GetInstances(itemType);
            var listGenericType = typeof(List<>);
            var list = listGenericType.MakeGenericType(itemType);
            var ci = list.GetConstructor([typeof(int)])!;
            var instance = (IList)ci.Invoke([items.Count(),]);
            foreach (var item in items)
            {
                instance.Add(item);
            }

            return instance;
        }
        else
        {
            return GetInstance(serviceType);
        }
    }

    public override string ToString()
    {
        return $"{(ShortAddress)Address}: {EndPointUtility.ToString(EndPoint)}";
    }

    public async Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning != true, "Node is not running.");

        _nodeInfo = await _remoteService.Server.GetInfoAsync(cancellationToken);
        return _nodeInfo;
    }

    public async Task StartAsync(NodeOptions nodeOptions, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning == true, "Node is already running.");

        _closeToken = await _remoteContext.OpenAsync(cancellationToken);
        _nodeInfo = await _remoteService.Server.StartAsync(nodeOptions, cancellationToken);
        _blocksyncEndPoint = DnsEndPointUtility.GetEndPoint(_nodeInfo.SwarmEndPoint);
        _consensusEndPoint = DnsEndPointUtility.GetEndPoint(_nodeInfo.ConsensusEndPoint);
        NodeOptions = nodeOptions;
        IsRunning = true;
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning != true, "Node is not running.");

        await _remoteService.Server.StopAsync(cancellationToken);
        await _remoteContext.CloseAsync(_closeToken, cancellationToken);
        NodeOptions = NodeOptions.Default;
        IsRunning = false;
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async Task<TxId> AddTransactionAsync(
        IAction[] actions, CancellationToken cancellationToken)
    {
        var address = Address.ToString();
        var values = actions.Select(item => item.PlainValue).ToArray();
        var nonce = await _remoteService.Server.GetNextNonceAsync(address, cancellationToken);
        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: _privateKey,
            genesisHash: BlockHash.FromString(_nodeInfo.GenesisHash),
            actions: new TxActionList(values)
        );

        var bytes = await _remoteService.Server.SendTransactionAsync(
            transaction: transaction.Serialize(),
            cancellationToken: cancellationToken);

        return new TxId(bytes);
    }

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _remoteContext.Disconnected -= RemoteContext_Disconnected;
        _remoteContext.Faulted -= RemoteContext_Faulted;
        await _remoteContext.ReleaseAsync(_closeToken);
        NodeOptions = NodeOptions.Default;
        IsRunning = false;
        _container.Dispose();
        _isDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
        GC.SuppressFinalize(this);
    }

    void INodeCallback.OnBlockAppended(BlockInfo blockInfo)
    {
        BlockAppended?.Invoke(this, new BlockEventArgs(blockInfo));
    }

    private object? GetInstance(Type serviceType)
    {
        var contractName = AttributedModelServices.GetContractName(serviceType);
        return _container.GetExportedValue<object>(contractName);
    }

    private IEnumerable<object> GetInstances(Type service)
    {
        var contractName = AttributedModelServices.GetContractName(service);
        return _container.GetExportedValues<object>(contractName);
    }

    private void RemoteContext_Disconnected(object? sender, EventArgs e)
    {
        NodeOptions = NodeOptions.Default;
        IsRunning = false;
        _isDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
    }

    private void RemoteContext_Faulted(object? sender, EventArgs e)
    {
        NodeOptions = NodeOptions.Default;
        IsRunning = false;
        _isDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
    }
}
