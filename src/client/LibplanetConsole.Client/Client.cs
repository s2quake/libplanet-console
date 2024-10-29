using Grpc.Net.Client;
using LibplanetConsole.Client.Services;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Grpc.Blockchain;
using LibplanetConsole.Grpc.Node;
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
    private IClientContent[]? _contents;

    public Client(ILogger<Client> logger, IApplicationOptions options)
    {
        _logger = logger;
        _privateKey = options.PrivateKey;
        _logger.LogDebug("Client is creating...: {Address}", options.PrivateKey.Address);
        _nodeEndPoint = options.NodeEndPoint;
        _info = new() { Address = options.PrivateKey.Address };
        PublicKey = options.PrivateKey.PublicKey;
        _logger.LogDebug("Client is created: {Address}", Address);
    }

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public PublicKey PublicKey { get; }

    public Address Address => PublicKey.Address;

    public TextWriter Out { get; set; } = Console.Out;

    public ClientInfo Info => _info;

    public IClientContent[] Contents
    {
        get => _contents ?? throw new InvalidOperationException("Contents is not initialized.");
        set => _contents = value;
    }

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

        if (_nodeEndPoint is null)
        {
            throw new InvalidOperationException($"{nameof(NodeEndPoint)} is not initialized.");
        }

        var channel = NodeChannel.CreateChannel(_nodeEndPoint);
        var nodeService = new NodeService(channel);
        var blockChainService = new BlockChainService(channel);
        nodeService.Started += (sender, e) => InvokeNodeStartedEvent(e);
        nodeService.Stopped += (sender, e) => InvokeNodeStoppedEvent();
        blockChainService.BlockAppended += BlockChainService_BlockAppended;
        try
        {
            await nodeService.InitializeAsync(cancellationToken);
            await blockChainService.InitializeAsync(cancellationToken);
        }
        catch
        {
            nodeService.Dispose();
            blockChainService.Dispose();
            throw;
        }

        _cancellationTokenSource = new();
        _channel = channel;
        _nodeService = nodeService;
        _blockChainService = blockChainService;
        _info = _info with
        {
            NodeInfo = nodeService.Info,
            Tip = nodeService.Info.Tip,
        };
        IsRunning = true;
        _logger.LogDebug("Client is started: {Address}->{NodeAddress}", Address, NodeInfo.Address);
        await Task.WhenAll(Contents.Select(item => item.StartAsync(cancellationToken)));
        _logger.LogDebug("Client Contents are started: {Address}", Address);
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException("The client is not running.");
        }

        await Task.WhenAll(Contents.Select(item => item.StopAsync(cancellationToken)));
        _logger.LogDebug("Client Contents are stopped: {Address}", Address);

        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
        }

        if (_nodeService is not null)
        {
            await _nodeService.ReleaseAsync(cancellationToken);
            _nodeService = null;
        }

        if (_blockChainService is not null)
        {
            await _blockChainService.ReleaseAsync(cancellationToken);
            _blockChainService = null;
        }

        await _channel.ShutdownAsync();
        _channel.Dispose();
        _channel = null;
        _blockChainService = null;
        _nodeService = null;
        IsRunning = false;
        _info = ClientInfo.Empty;
        _logger.LogDebug("Client is stopped: {Address}", Address);
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public void InvokeNodeStartedEvent(NodeEventArgs e)
    {
        NodeInfo = e.NodeInfo;
        _info = _info with { NodeInfo = e.NodeInfo };
    }

    public void InvokeNodeStoppedEvent()
    {
        NodeInfo = NodeInfo.Empty;
        _info = _info with { NodeInfo = default };
    }

    public void InvokeBlockAppendedEvent(BlockEventArgs e)
        => BlockAppended?.Invoke(this, e);

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
            Stopped?.Invoke(this, EventArgs.Empty);
        }
    }

    private void BlockChainService_BlockAppended(object? sender, BlockEventArgs e)
    {
        _info = _info with
        {
            Tip = e.BlockInfo,
        };
        BlockAppended?.Invoke(this, e);
    }
}
