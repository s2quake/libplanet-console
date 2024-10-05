using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Node;
using LibplanetConsole.Node.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Console;

internal sealed partial class Node : INode, IBlockChain, INodeCallback, IBlockChainCallback
{
    private static readonly Codec _codec = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly NodeOptions _nodeOptions;
    // private readonly RemoteService<INodeService, INodeCallback> _remoteService;
    // private readonly RemoteService<IBlockChainService, IBlockChainCallback> _blockChainService;
    private readonly ILogger _logger;
    private readonly CancellationTokenSource _processCancellationTokenSource = new();
    // private RemoteServiceContext? _remoteServiceContext;
    private EndPoint? _blocksyncEndPoint;
    private EndPoint? _consensusEndPoint;
    private Guid _closeToken;
    private NodeInfo _nodeInfo;
    private bool _isDisposed;
    private bool _isInProgress;
    private NodeProcess? _process;
    private Task _processTask = Task.CompletedTask;

    public Node(IServiceProvider serviceProvider, NodeOptions nodeOptions)
    {
        _serviceProvider = serviceProvider;
        _nodeOptions = nodeOptions;
        _logger = _serviceProvider.GetLogger<Node>();
        // _remoteService = new(this);
        // _blockChainService = new(this);
        PublicKey = nodeOptions.PrivateKey.PublicKey;
        _logger.LogDebug("Node is created: {Address}", Address);
    }

    public event EventHandler? Attached;

    public event EventHandler? Detached;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public event EventHandler? Disposed;

    public EndPoint SwarmEndPoint
        => _blocksyncEndPoint ?? throw new InvalidOperationException("Peer is not set.");

    public EndPoint ConsensusEndPoint
        => _consensusEndPoint ?? throw new InvalidOperationException("ConsensusPeer is not set.");

    public PublicKey PublicKey { get; }

    public Address Address => PublicKey.Address;

    public bool IsAttached => _closeToken != Guid.Empty;

    public bool IsRunning { get; private set; }

    public EndPoint EndPoint => _nodeOptions.EndPoint;

    public NodeInfo Info => _nodeInfo;

    public object? GetService(Type serviceType) => _serviceProvider.GetService(serviceType);

    public override string ToString() => $"{Address.ToShortString()}: {EndPoint}";

    public byte[] Sign(object obj) => _nodeOptions.PrivateKey.Sign(obj);

    public async Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning != true, "Node is not running.");

        // _nodeInfo = await _remoteService.Service.GetInfoAsync(cancellationToken);
        // return _nodeInfo;
        throw new NotImplementedException();
    }

    public async Task AttachAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: _closeToken != Guid.Empty,
            message: "Node is already attached.");

        // if (_remoteServiceContext is not null)
        // {
        //     throw new InvalidOperationException("Node is already attached.");
        // }

        // _remoteServiceContext = new RemoteServiceContext(
        //     [_remoteService, _blockChainService, .. GetRemoteServices(_serviceProvider)])
        // {
        //     EndPoint = _nodeOptions.EndPoint,
        // };
        // _remoteServiceContext.Closed += RemoteServiceContext_Closed;

        using var scope = new ProgressScope(this);
        // _closeToken = await _remoteServiceContext.OpenAsync(cancellationToken);
        // _nodeInfo = await _remoteService.Service.GetInfoAsync(cancellationToken);
        IsRunning = _nodeInfo.IsRunning;
        _logger.LogDebug("Node is attached: {Address}", Address);
        Attached?.Invoke(this, EventArgs.Empty);
    }

    public async Task DetachAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: _closeToken == Guid.Empty,
            message: "Node is not attached.");

        // if (_remoteServiceContext is null)
        // {
        //     throw new InvalidOperationException("Node is not attached.");
        // }

        using var scope = new ProgressScope(this);
        // _remoteServiceContext.Closed -= RemoteServiceContext_Closed;
        // await _remoteServiceContext.CloseAsync(_closeToken, cancellationToken);
        // _remoteServiceContext = null;
        _closeToken = Guid.Empty;
        _logger.LogDebug("Node is detached: {Address}", Address);
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
        var application = this.GetRequiredService<ApplicationBase>();
        var seedEndPoint = EndPointUtility.ToString(
            _nodeOptions.SeedEndPoint ?? application.Info.EndPoint);
        // _nodeInfo = await _remoteService.Service.StartAsync(seedEndPoint, cancellationToken);
        _blocksyncEndPoint = EndPointUtility.Parse(_nodeInfo.SwarmEndPoint);
        _consensusEndPoint = EndPointUtility.Parse(_nodeInfo.ConsensusEndPoint);
        IsRunning = true;
        _logger.LogDebug("Node is started: {Address}", Address);
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
        // await _remoteService.Service.StopAsync(cancellationToken);
        _closeToken = Guid.Empty;
        _nodeInfo = NodeInfo.Empty;
        IsRunning = false;
        _logger.LogDebug("Node is stopped: {Address}", Address);
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed is false)
        {
            await _processCancellationTokenSource.CancelAsync();
            _processCancellationTokenSource.Dispose();
            await _processTask;
            _processTask = Task.CompletedTask;
            _process = null;

            // if (_remoteServiceContext is not null)
            // {
            //     _remoteServiceContext.Closed -= RemoteServiceContext_Closed;
            //     await _remoteServiceContext.CloseAsync(_closeToken);
            //     _remoteServiceContext = null;
            // }

            _closeToken = Guid.Empty;
            IsRunning = false;
            _isDisposed = true;
            _logger.LogDebug("Node is disposed: {Address}", Address);
            Disposed?.Invoke(this, EventArgs.Empty);
            GC.SuppressFinalize(this);
        }
    }

    public Task StartProcessAsync(AddNewNodeOptions options, CancellationToken cancellationToken)
    {
        if (_process is not null)
        {
            throw new InvalidOperationException("Node process is already running.");
        }

        var nodeOptions = _nodeOptions;
        var process = new NodeProcess(this, nodeOptions)
        {
            Detach = options.Detach,
            NewWindow = options.NewWindow,
        };
        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken, _processCancellationTokenSource.Token);
        _logger.LogDebug(process.ToString());
        _processTask = process.RunAsync(cancellationTokenSource.Token)
            .ContinueWith(
                task =>
                {
                    _processTask = Task.CompletedTask;
                    _process = null;
                    cancellationTokenSource.Dispose();
                },
                cancellationToken);
        _process = process;
        return Task.CompletedTask;
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

    // private static IEnumerable<IRemoteService> GetRemoteServices(
    //     IServiceProvider serviceProvider)
    // {
    //     foreach (var item in serviceProvider.GetServices<INodeContentService>())
    //     {
    //         yield return item.RemoteService;
    //     }
    // }

    // private void RemoteServiceContext_Closed(object? sender, EventArgs e)
    // {
    //     if (sender is RemoteServiceContext remoteServiceContext)
    //     {
    //         remoteServiceContext.Closed -= RemoteServiceContext_Closed;
    //         _remoteServiceContext = null;
    //         if (_isInProgress != true && IsRunning == true)
    //         {
    //             _closeToken = Guid.Empty;
    //             Detached?.Invoke(this, EventArgs.Empty);
    //         }
    //     }
    // }

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
