using Grpc.Net.Client;
using LibplanetConsole.Client.Grpc;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Node;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Client;

internal sealed partial class Client : IClient
{
    private readonly ILogger _logger;
    private readonly PrivateKey _privateKey;
    private EndPoint? _nodeEndPoint;
    private NodeService? _nodeService;
    private BlockChainService? _blockChainService;
    private GrpcChannel? _channel;
    private CancellationTokenSource? _cancellationTokenSource;
    private ClientInfo _info;

    public Client(ILogger<Client> logger, ApplicationOptions options)
    {
        _logger = logger;
        _privateKey = options.PrivateKey;
        _logger.LogDebug("Client is creating...: {Address}", options.PrivateKey.Address);
        _nodeEndPoint = options.NodeEndPoint;
        _info = new() { Address = options.PrivateKey.Address };
        PublicKey = options.PrivateKey.PublicKey;
        _logger.LogDebug("Client is created: {Address}", Address);
    }

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public event EventHandler? Started;

    public event EventHandler<StopEventArgs>? Stopped;

    public PublicKey PublicKey { get; }

    public Address Address => PublicKey.Address;

    public TextWriter Out { get; set; } = Console.Out;

    public ClientInfo Info => _info;

    public NodeInfo NodeInfo { get; private set; }

    public EndPoint NodeEndPoint
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

    public override string ToString() => $"[{Address}]";

    public bool Verify(object obj, byte[] signature) => PublicKey.Verify(obj, signature);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            throw new InvalidOperationException("The client is already running.");
        }

        await Task.Delay(1000);

        var address = $"http://{EndPointUtility.ToString(_nodeEndPoint)}";
        var channelOptions = new GrpcChannelOptions
        {
            ThrowOperationCanceledOnCancellation = true,
            MaxRetryAttempts = 1,
        };
        _channel = GrpcChannel.ForAddress(address, channelOptions);
        _cancellationTokenSource = new();
        _nodeService = new NodeService(_channel);
        _nodeService.Disconnected += NodeService_Disconnected;
        _nodeService.Started += (sender, e) => InvokeNodeStartedEvent(e);
        _nodeService.Stopped += (sender, e) => InvokeNodeStoppedEvent();
        _blockChainService = new BlockChainService(_channel);
        _blockChainService.BlockAppended += (sender, e) => InvokeBlockAppendedEvent(e);
        await Task.WhenAll(
            _nodeService.StartAsync(cancellationToken),
            _blockChainService.StartAsync(cancellationToken));
        _info = _info with { NodeAddress = NodeInfo.Address };
        IsRunning = true;
        _logger.LogDebug(
            "Client is started: {Address} -> {NodeAddress}", Address, NodeInfo.Address);
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException("The client is not running.");
        }

        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
        }

        if (_nodeService is not null)
        {
            _nodeService.Disconnected -= NodeService_Disconnected;
            await _nodeService.StopAsync(cancellationToken);
            _nodeService = null;
        }

        if (_blockChainService is not null)
        {
            await _blockChainService.StopAsync(cancellationToken);
            _blockChainService = null;
        }

        await _channel.ShutdownAsync();
        _channel.Dispose();
        _channel = null;
        _blockChainService = null;
        _nodeService = null;
        IsRunning = false;
        _info = _info with { NodeAddress = default };
        _logger.LogDebug("Client is stopped: {Address}", Address);
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
        _nodeService?.Dispose();
        _nodeService = null;
        _blockChainService?.Dispose();
        _blockChainService = null;
        _channel?.Dispose();
        _channel = null;
        IsRunning = false;
        await ValueTask.CompletedTask;
    }

    private void NodeService_Disconnected(object? sender, EventArgs e)
    {
        if (_cancellationTokenSource?.IsCancellationRequested is false)
        {
            _nodeService?.Dispose();
            _nodeService = null;
            _blockChainService?.Dispose();
            _blockChainService = null;
            _channel?.Dispose();
            _channel = null;
            IsRunning = false;
            Stopped?.Invoke(this, new(StopReason.None));
        }
    }
}
