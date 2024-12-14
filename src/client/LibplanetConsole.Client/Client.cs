using Grpc.Net.Client;
using LibplanetConsole.Client.Services;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Grpc.Node;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Client;

internal sealed class Client : IClient
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    private readonly PrivateKey _privateKey;
    private EndPoint? _nodeEndPoint;
    private NodeService? _nodeService;
    private GrpcChannel? _channel;
    private CancellationTokenSource? _cancellationTokenSource;
    private ClientInfo _info;
    private IClientContent[]? _contents;

    public Client(
        IServiceProvider serviceProvider, IApplicationOptions options)
    {
        _serviceProvider = serviceProvider;
        _logger = serviceProvider.GetRequiredService<ILogger<Client>>();
        _privateKey = options.PrivateKey;
        _logger.LogDebug("Client is creating...: {Address}", options.PrivateKey.Address);
        _nodeEndPoint = options.NodeEndPoint;
        _info = ClientInfo.Empty with { Address = options.PrivateKey.Address };
        PublicKey = options.PrivateKey.PublicKey;
        _logger.LogDebug("Client is created: {Address}", Address);
        _logger.LogDebug(JsonUtility.Serialize(Info));
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
        var nodeService = await NodeService.CreateAsync(channel, cancellationToken);

        _logger.LogDebug(JsonUtility.Serialize(nodeService.Info));
        _cancellationTokenSource = new();
        _channel = channel;
        _nodeService = nodeService;
        IsRunning = true;
        _info = _info with
        {
            GenesisHash = nodeService.Info.GenesisHash,
            IsRunning = IsRunning,
        };
        _logger.LogDebug(JsonUtility.Serialize(_info));
        _logger.LogDebug(
            "Client is started: {Address}->{NodeAddress}", Address, nodeService.Info.Address);
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

        await _channel.ShutdownAsync();
        _channel.Dispose();
        _channel = null;
        _nodeService = null;
        IsRunning = false;
        _info = ClientInfo.Empty;
        _logger.LogDebug("Client is stopped: {Address}", Address);
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async Task<TxId> SendTransactionAsync(
        IAction[] actions, CancellationToken cancellationToken)
    {
        var blockChain = _serviceProvider.GetRequiredService<ClientBlockChain>();
        var address = _privateKey.Address;
        var nonce = await blockChain.GetNextNonceAsync(address, cancellationToken);
        var genesisHash = _info.GenesisHash;
        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: _privateKey,
            genesisHash: genesisHash,
            actions: [.. actions.Select(item => item.PlainValue)]);
        await blockChain.SendTransactionAsync(transaction, cancellationToken);
        return transaction.Id;
    }

    public async ValueTask DisposeAsync()
    {
        _nodeService?.Dispose();
        _nodeService = null;
        _channel?.Dispose();
        _channel = null;
        IsRunning = false;
        await ValueTask.CompletedTask;
    }
}
