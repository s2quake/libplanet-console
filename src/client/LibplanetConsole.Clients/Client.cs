using System.Security;
using LibplanetConsole.Clients.Services;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Nodes;
using LibplanetConsole.Nodes.Services;
using Serilog;

namespace LibplanetConsole.Clients;

internal sealed partial class Client : IClient, INodeCallback, IBlockChainCallback
{
    private readonly ApplicationBase _application;
    private readonly SecureString _privateKey;
    private readonly ILogger _logger;
    private AppEndPoint? _nodeEndPoint;
    private RemoteNodeContext? _remoteNodeContext;
    private Guid _closeToken;
    private ClientInfo _info;

    public Client(ApplicationBase application, ApplicationOptions options)
    {
        _logger = application.Logger;
        _logger.Debug("Client is creating...: {Address}", options.PrivateKey.Address);
        _application = application;
        _nodeEndPoint = options.NodeEndPoint;
        _privateKey = options.PrivateKey.ToSecureString();
        _info = new() { Address = options.PrivateKey.Address };
        PublicKey = options.PrivateKey.PublicKey;
        _logger.Debug("Client is created: {Address}", Address);
    }

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public event EventHandler? Started;

    public event EventHandler<StopEventArgs>? Stopped;

    public AppPublicKey PublicKey { get; }

    public AppAddress Address => PublicKey.Address;

    public TextWriter Out { get; set; } = Console.Out;

    public ClientInfo Info => _info;

    public NodeInfo NodeInfo { get; private set; }

    public AppEndPoint NodeEndPoint
    {
        get => _nodeEndPoint ??
            throw new InvalidOperationException($"{nameof(NodeEndPoint)} is not initialized.");
        set
        {
            if (IsRunning == true)
            {
                throw new InvalidOperationException("The client is running.");
            }

            _nodeEndPoint = value;
        }
    }

    public bool IsRunning { get; private set; }

    private INodeService RemoteNodeService => _application.GetService<RemoteNodeService>().Service;

    private IBlockChainService RemoteBlockChainService
        => _application.GetService<RemoteBlockChainService>().Service;

    public override string ToString() => $"[{Address}]";

    public bool Verify(object obj, byte[] signature) => PublicKey.Verify(obj, signature);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_remoteNodeContext is not null)
        {
            throw new InvalidOperationException("The client is already running.");
        }

        _remoteNodeContext = _application.GetService<RemoteNodeContext>();
        _remoteNodeContext.EndPoint = NodeEndPoint;
        _closeToken = await _remoteNodeContext.OpenAsync(cancellationToken);
        _remoteNodeContext.Closed += RemoteNodeContext_Closed;
        NodeInfo = await RemoteNodeService.GetInfoAsync(cancellationToken);
        _info = _info with { NodeAddress = NodeInfo.Address };
        IsRunning = true;
        _logger.Debug("Client is started: {Address} -> {NodeAddress}", Address, NodeInfo.Address);
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_remoteNodeContext is null)
        {
            throw new InvalidOperationException("The client is not running.");
        }

        _remoteNodeContext.Closed -= RemoteNodeContext_Closed;
        await _remoteNodeContext.CloseAsync(_closeToken, cancellationToken);
        _info = _info with { NodeAddress = default };
        _remoteNodeContext = null;
        _closeToken = Guid.Empty;
        IsRunning = false;
        _logger.Debug("Client is stopped: {Address}", Address);
        Stopped?.Invoke(this, new(StopReason.None));
    }

    public void InvokeNodeStartedEvent(NodeInfo nodeInfo)
    {
        NodeInfo = nodeInfo;
        _info = _info with { NodeAddress = NodeInfo.Address };
    }

    public void InvokeNodeStoppedEvent()
    {
        NodeInfo = NodeInfo.Empty;
        _info = _info with { NodeAddress = default };
    }

    public void InvokeBlockAppendedEvent(BlockInfo blockInfo)
        => BlockAppended?.Invoke(this, new BlockEventArgs(blockInfo));

    public async ValueTask DisposeAsync()
    {
        if (_remoteNodeContext is not null)
        {
            _remoteNodeContext.Closed -= RemoteNodeContext_Closed;
            await _remoteNodeContext.CloseAsync(_closeToken);
            _remoteNodeContext = null;
        }
    }

    void INodeCallback.OnStarted(NodeInfo nodeInfo) => NodeInfo = nodeInfo;

    void INodeCallback.OnStopped() => NodeInfo = NodeInfo.Empty;

    void IBlockChainCallback.OnBlockAppended(BlockInfo blockInfo)
    {
        BlockAppended?.Invoke(this, new BlockEventArgs(blockInfo));
    }

    private void RemoteNodeContext_Closed(object? sender, EventArgs e)
    {
        if (_remoteNodeContext is not null)
        {
            _remoteNodeContext.Closed -= RemoteNodeContext_Closed;
            _remoteNodeContext = null;
        }

        _closeToken = Guid.Empty;
        IsRunning = false;
        Stopped?.Invoke(this, new(StopReason.None));
    }
}
