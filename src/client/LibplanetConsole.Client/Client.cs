using System.Security;
using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Node;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Client;

internal sealed partial class Client : IClient
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SecureString _privateKey;
    private readonly ILogger _logger;
    private EndPoint? _nodeEndPoint;
    private Node.Grpc.NodeGrpcService.NodeGrpcServiceClient? _remoteNode;
    private Node.Grpc.BlockChainGrpcService.BlockChainGrpcServiceClient? _remoteBlockchain;
    private GrpcChannel? _channel;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task _callbackTask = Task.CompletedTask;
    private Guid _closeToken;
    private ClientInfo _info;

    public Client(IServiceProvider serviceProvider, ApplicationOptions options)
    {
        _serviceProvider = serviceProvider;
        _logger = serviceProvider.GetLogger<Client>();
        _logger.LogDebug("Client is creating...: {Address}", options.PrivateKey.Address);
        _nodeEndPoint = options.NodeEndPoint;
        _privateKey = options.PrivateKey.ToSecureString();
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

        var address = $"http://{EndPointUtility.ToString(_nodeEndPoint)}";
        var channelOptions = new GrpcChannelOptions
        {
            ThrowOperationCanceledOnCancellation = true,
            MaxRetryAttempts = 1,
        };
        HubConnectionContext
        _channel = GrpcChannel.ForAddress(address, channelOptions);
        _cancellationTokenSource = new();
        _remoteNode = new Node.Grpc.NodeGrpcService.NodeGrpcServiceClient(_channel);
        _remoteBlockchain = new Node.Grpc.BlockChainGrpcService.BlockChainGrpcServiceClient(_channel);
        _callbackTask = CallbackAsync(_cancellationTokenSource.Token);

        _channel.Target

        // _remoteNodeContext = _serviceProvider.GetRequiredService<RemoteNodeContext>();
        // _remoteNodeContext.EndPoint = NodeEndPoint;
        // _closeToken = await _remoteNodeContext.OpenAsync(cancellationToken);
        // _remoteNodeContext.Closed += RemoteNodeContext_Closed;
        // NodeInfo = await RemoteNodeService.GetInfoAsync(cancellationToken);
        _info = _info with { NodeAddress = NodeInfo.Address };
        IsRunning = true;
        _logger.LogDebug(
            "Client is started: {Address} -> {NodeAddress}", Address, NodeInfo.Address);
        Started?.Invoke(this, EventArgs.Empty);
    }

    private async Task CallbackAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(2000, cancellationToken);
            var callOptions = new CallOptions(deadline: DateTime.UtcNow.AddSeconds(1), cancellationToken: cancellationToken);
            var response1 = await _remoteBlockchain.IsReadyAsync(new Node.Grpc.IsReadyRequest(), callOptions);
            if (response1.IsReady is false)
            {
                throw new InvalidOperationException("The remote node is not ready.");
            }

            using var streamingCall = _remoteBlockchain.GetBlockAppendedStream(
                new Node.Grpc.GetBlockAppendedStreamRequest(),
                deadline: DateTime.UtcNow + TimeSpan.FromSeconds(10),
                cancellationToken: cancellationToken);

            while (await streamingCall.ResponseStream.MoveNext(cancellationToken))
            {
                var response = streamingCall.ResponseStream.Current;
                InvokeBlockAppendedEvent(response.BlockInfo);
            }
        }
        catch (Exception e)
        {
            int qwer = 0;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException("The client is not running.");
        }

        // _remoteNodeContext.Closed -= RemoteNodeContext_Closed;
        // await _remoteNodeContext.CloseAsync(_closeToken, cancellationToken);
        // _info = _info with { NodeAddress = default };
        // _remoteNodeContext = null;
        await _channel.ShutdownAsync();
        _channel.Dispose();
        _channel = null;
        _closeToken = Guid.Empty;
        IsRunning = false;
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
        // if (_remoteNodeContext is not null)
        // {
        //     _remoteNodeContext.Closed -= RemoteNodeContext_Closed;
        //     await _remoteNodeContext.CloseAsync(_closeToken);
        //     _remoteNodeContext = null;
        // }
    }

    // void INodeCallback.OnStarted(NodeInfo nodeInfo) => NodeInfo = nodeInfo;

    // void INodeCallback.OnStopped() => NodeInfo = NodeInfo.Empty;

    // void IBlockChainCallback.OnBlockAppended(BlockInfo blockInfo)
    // {
    //     BlockAppended?.Invoke(this, new BlockEventArgs(blockInfo));
    // }

    private void RemoteNodeContext_Closed(object? sender, EventArgs e)
    {
        // if (_remoteNodeContext is not null)
        // {
        //     _remoteNodeContext.Closed -= RemoteNodeContext_Closed;
        //     _remoteNodeContext = null;
        // }

        _closeToken = Guid.Empty;
        IsRunning = false;
        Stopped?.Invoke(this, new(StopReason.None));
    }
}
