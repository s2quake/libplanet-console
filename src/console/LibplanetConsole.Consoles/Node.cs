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
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Nodes.Serializations;
using LibplanetConsole.Nodes.Services;
using Serilog;

namespace LibplanetConsole.Consoles;

internal sealed class Node
    : INodeCallback, IAsyncDisposable, IAddressable, INode
{
    private readonly ApplicationContainer _container;
    private readonly SecureString _privateKey;
    private readonly RemoteServiceContext _remoteServiceContext;
    private readonly RemoteService<INodeService, INodeCallback> _remoteService;
    private readonly INodeContent[] _contents;
    private readonly ILogger _logger;
    private DnsEndPoint? _blocksyncEndPoint;
    private DnsEndPoint? _consensusEndPoint;
    private Guid _closeToken;
    private NodeInfo _nodeInfo;
    private bool _isDisposed;
    private bool _isInProgress;

    public Node(ApplicationBase application, PrivateKey privateKey, NodeOptions nodeOptions)
    {
        _container = application.CreateChildContainer(this);
        _privateKey = PrivateKeyUtility.ToSecureString(privateKey);
        _container.ComposeExportedValue<INode>(this);
        _contents = [.. _container.GetExportedValues<INodeContent>()];
        _logger = application.GetService<ILogger>();
        _remoteService = new(this);
        _remoteServiceContext = new RemoteServiceContext(
            [_remoteService, .. GetRemoteServices(_container)]);
        PublicKey = privateKey.PublicKey;
        NodeOptions = nodeOptions;
        _remoteServiceContext.Closed += RemoteServiceContext_Closed;
        _logger.Debug("Node is created: {Address}", Address);
    }

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public event EventHandler? Attached;

    public event EventHandler? Detached;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public event EventHandler? Disposed;

    public DnsEndPoint SwarmEndPoint
        => _blocksyncEndPoint ?? throw new InvalidOperationException("Peer is not set.");

    public DnsEndPoint ConsensusEndPoint
        => _consensusEndPoint ?? throw new InvalidOperationException("ConsensusPeer is not set.");

    public PublicKey PublicKey { get; }

    public Address Address => PublicKey.Address;

    public bool IsAttached => _closeToken != Guid.Empty;

    public bool IsRunning { get; private set; }

    public NodeOptions NodeOptions { get; }

    public EndPoint EndPoint
    {
        get => _remoteServiceContext.EndPoint;
        set => _remoteServiceContext.EndPoint = value;
    }

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

    public async Task AttachAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: _closeToken != Guid.Empty,
            message: "Node is already attached.");

        using var scope = new ProgressScope(this);
        _closeToken = await _remoteServiceContext.OpenAsync(cancellationToken);
        _nodeInfo = await _remoteService.Service.GetInfoAsync(cancellationToken);
        IsRunning = _nodeInfo.IsRunning;
        _logger.Debug("Node is attached: {Address}", Address);
        Attached?.Invoke(this, EventArgs.Empty);
    }

    public async Task DetachAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: _closeToken == Guid.Empty,
            message: "Node is not attached.");

        using var scope = new ProgressScope(this);
        await _remoteServiceContext.CloseAsync(_closeToken, cancellationToken);
        _closeToken = Guid.Empty;
        _logger.Debug("Node is detached: {Address}", Address);
        Detached?.Invoke(this, EventArgs.Empty);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning == true, "Node is already running.");
        InvalidOperationExceptionUtility.ThrowIf(
            condition: _closeToken == Guid.Empty,
            message: "Node is not attached.");

        using var scope = new ProgressScope(this);
        _nodeInfo = await _remoteService.Service.StartAsync(cancellationToken);
        _blocksyncEndPoint = DnsEndPointUtility.Parse(_nodeInfo.SwarmEndPoint);
        _consensusEndPoint = DnsEndPointUtility.Parse(_nodeInfo.ConsensusEndPoint);
        IsRunning = true;
        _logger.Debug("Node is started: {Address}", Address);
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning != true, "Node is not running.");
        InvalidOperationExceptionUtility.ThrowIf(
            condition: _closeToken == Guid.Empty,
            message: "Node is not attached.");

        using var scope = new ProgressScope(this);
        await _remoteService.Service.StopAsync(cancellationToken);
        _closeToken = Guid.Empty;
        _nodeInfo = NodeInfo.Empty;
        IsRunning = false;
        _logger.Debug("Node is stopped: {Address}", Address);
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
        if (_closeToken != Guid.Empty)
        {
            await _remoteServiceContext.CloseAsync(_closeToken);
            _closeToken = Guid.Empty;
        }

        IsRunning = false;
        await _container.DisposeAsync();
        _isDisposed = true;
        _logger.Debug("Node is disposed: {Address}", Address);
        Disposed?.Invoke(this, EventArgs.Empty);
        GC.SuppressFinalize(this);
    }

    public NodeProcess CreateProcess()
    {
        var endPoint = EndPoint;
        var application = IServiceProviderExtensions.GetService<ApplicationBase>(this);
        return new NodeProcess
        {
            EndPoint = endPoint,
            PrivateKey = _privateKey,
            NodeEndPoint = EndPointUtility.Parse(application.Info.EndPoint),
            StoreDirectory = application.Info.StoreDirectory,
            LogDirectory = application.Info.LogDirectory,
        };
    }

    void INodeCallback.OnStarted(NodeInfo nodeInfo)
    {
        if (_isInProgress != true)
        {
            _nodeInfo = nodeInfo;
            IsRunning = true;
            Started?.Invoke(this, EventArgs.Empty);
        }
    }

    void INodeCallback.OnStopped()
    {
        if (_isInProgress != true)
        {
            _nodeInfo = NodeInfo.Empty;
            IsRunning = false;
            Stopped?.Invoke(this, EventArgs.Empty);
        }
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
        if (_isInProgress != true && IsRunning == true)
        {
            _closeToken = Guid.Empty;
            Detached?.Invoke(this, EventArgs.Empty);
        }
    }

    private sealed class ProgressScope : IDisposable
    {
        private readonly Node _node;

        public ProgressScope(Node node)
        {
            _node = node;
            _node._isInProgress = true;
        }

        public void Dispose()
        {
            _node._isInProgress = false;
        }
    }
}
