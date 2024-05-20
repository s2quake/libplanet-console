using System.Collections;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Net;
using System.Security;
using Libplanet.Action;
using Libplanet.Crypto;
using Libplanet.Types.Tx;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Nodes;
using LibplanetConsole.Nodes.Serializations;
using LibplanetConsole.Nodes.Services;

namespace LibplanetConsole.Consoles;

internal sealed class Node
    : INodeCallback, IAsyncDisposable, IAddressable, INode
{
    private readonly CompositionContainer _container;
    private readonly SecureString _privateKey;
    private readonly RemoteServiceContext _remoteServiceContext;
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
        _privateKey = PrivateKeyUtility.ToSecureString(privateKey);
        _container.ComposeExportedValue<INode>(this);
        _contents = [.. _container.GetExportedValues<INodeContent>()];
        _remoteService = new(this);
        _remoteServiceContext = new RemoteServiceContext(
            [_remoteService, .. GetRemoteServices(container)])
        {
            EndPoint = endPoint,
        };
        PublicKey = privateKey.PublicKey;
        _remoteServiceContext.Closed += RemoteServiceContext_Closed;
    }

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public event EventHandler? Disposed;

    public DnsEndPoint SwarmEndPoint
        => _blocksyncEndPoint ?? throw new InvalidOperationException("Peer is not set.");

    public DnsEndPoint ConsensusEndPoint
        => _consensusEndPoint ?? throw new InvalidOperationException("ConsensusPeer is not set.");

    public PublicKey PublicKey { get; }

    public Address Address => PublicKey.Address;

    public bool IsRunning { get; private set; }

    public NodeOptions NodeOptions { get; private set; } = NodeOptions.Default;

    public EndPoint EndPoint => _remoteServiceContext.EndPoint;

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

    public byte[] Sign(object obj)
    {
        var privateKey = PrivateKeyUtility.FromSecureString(_privateKey);
        return PrivateKeyUtility.Sign(privateKey, obj);
    }

    public async Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning != true, "Node is not running.");

        _nodeInfo = await _remoteService.Service.GetInfoAsync(cancellationToken);
        return _nodeInfo;
    }

    public async Task StartAsync(NodeOptions nodeOptions, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning == true, "Node is already running.");

        _closeToken = await _remoteServiceContext.OpenAsync(cancellationToken);
        _nodeInfo = await _remoteService.Service.StartAsync(nodeOptions, cancellationToken);
        _blocksyncEndPoint = DnsEndPointUtility.Parse(_nodeInfo.SwarmEndPoint);
        _consensusEndPoint = DnsEndPointUtility.Parse(_nodeInfo.ConsensusEndPoint);
        NodeOptions = nodeOptions;
        IsRunning = true;
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning != true, "Node is not running.");

        await _remoteService.Service.StopAsync(cancellationToken);
        await _remoteServiceContext.CloseAsync(_closeToken, cancellationToken);
        NodeOptions = NodeOptions.Default;
        IsRunning = false;
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken)
        => _remoteService.Service.GetNextNonceAsync(address, cancellationToken);

    public async Task<TxId> SendTransactionAsync(
        IAction[] actions, CancellationToken cancellationToken)
    {
        var privateKey = PrivateKeyUtility.FromSecureString(_privateKey);
        var address = privateKey.Address;
        var nonce = await _remoteService.Service.GetNextNonceAsync(address, cancellationToken);
        var genesisHash = _nodeInfo.GenesisHash;
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: genesisHash,
            actions: [.. actions.Select(item => item.PlainValue)]
        );
        var txId = await _remoteService.Service.SendTransactionAsync(
            transaction: tx.Serialize(),
            cancellationToken: cancellationToken);

        return txId;
    }

    public Task<TxId> SendTransactionAsync(
        Transaction transaction, CancellationToken cancellationToken)
    {
        return _remoteService.Service.SendTransactionAsync(
            transaction.Serialize(), cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _remoteServiceContext.Closed -= RemoteServiceContext_Closed;
        await _remoteServiceContext.CloseAsync(_closeToken);
        NodeOptions = NodeOptions.Default;
        IsRunning = false;
        _container.Dispose();
        _isDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
        GC.SuppressFinalize(this);
    }

    void INodeCallback.OnStarted(NodeInfo nodeInfo)
    {
        _nodeInfo = nodeInfo;
    }

    void INodeCallback.OnStopped()
    {
        _nodeInfo = new();
    }

    void INodeCallback.OnBlockAppended(BlockInfo blockInfo)
    {
        _nodeInfo = _nodeInfo with { TipHash = blockInfo.Hash };
        BlockAppended?.Invoke(this, new BlockEventArgs(blockInfo));
    }

    private static IEnumerable<IRemoteService> GetRemoteServices(
        CompositionContainer compositionContainer)
    {
        foreach (var item in compositionContainer.GetExportedValues<INodeContentService>())
        {
            yield return item.RemoteService;
        }
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

    private void RemoteServiceContext_Closed(object? sender, EventArgs e)
    {
        NodeOptions = NodeOptions.Default;
        IsRunning = false;
        _isDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
    }
}
