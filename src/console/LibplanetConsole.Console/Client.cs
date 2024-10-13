using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Blockchain;
using LibplanetConsole.Blockchain.Grpc;
using LibplanetConsole.Client;
using LibplanetConsole.Client.Grpc;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.Threading;
using LibplanetConsole.Console.Grpc;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Console;

internal sealed partial class Client : IClient, IBlockChain
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ClientOptions _clientOptions;
    private readonly PrivateKey _privateKey;
    private readonly ILogger _logger;
    private readonly CancellationTokenSource _processCancellationTokenSource = new();
    private ClientService? _clientService;
    private BlockChainService? _blockChainService;
    private GrpcChannel? _channel;
    private ClientInfo _clientInfo;
    private bool _isDisposed;
    private ClientProcess? _process;
    private Task _processTask = Task.CompletedTask;
    private IClientContent[]? _contents;

    public Client(IServiceProvider serviceProvider, ClientOptions clientOptions)
    {
        _serviceProvider = serviceProvider;
        _privateKey = clientOptions.PrivateKey;
        _clientOptions = clientOptions;
        _logger = _serviceProvider.GetLogger<Client>();
        PublicKey = clientOptions.PrivateKey.PublicKey;
        _logger.LogDebug("Client is created: {Address}", Address);
    }

    public event EventHandler? Attached;

    public event EventHandler? Detached;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public event EventHandler? Disposed;

    public PublicKey PublicKey { get; }

    public Address Address => PublicKey.Address;

    public bool IsAttached { get; private set; }

    public bool IsRunning { get; private set; }

    public EndPoint EndPoint => _clientOptions.EndPoint;

    public ClientInfo Info => _clientInfo;

    public IClientContent[] Contents
    {
        get => _contents ?? throw new InvalidOperationException("Contents is not initialized.");
        set => _contents = value;
    }

    public object? GetService(Type serviceType) => _serviceProvider.GetService(serviceType);

    public override string ToString() => $"{Address.ToShortString()}: {EndPoint}";

    public byte[] Sign(object obj) => _clientOptions.PrivateKey.Sign(obj);

    public async Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_clientService is null)
        {
            throw new InvalidOperationException("Client is not attached.");
        }

        var request = new GetInfoRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _clientService.GetInfoAsync(request, callOptions);
        _clientInfo = response.ClientInfo;
        return _clientInfo;
    }

    public async Task AttachAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        if (_channel is not null)
        {
            throw new InvalidOperationException("Client is already attached.");
        }

        var channel = ClientChannel.CreateChannel(_clientOptions.EndPoint);
        var clientService = new ClientService(channel);
        var blockChainService = new BlockChainService(channel);
        clientService.Started += ClientService_Started;
        clientService.Stopped += ClientService_Stopped;
        blockChainService.BlockAppended += BlockChainService_BlockAppended;
        try
        {
            await clientService.StartAsync(cancellationToken);
            await blockChainService.InitializeAsync(cancellationToken);
        }
        catch
        {
            clientService.Dispose();
            blockChainService.Dispose();
            throw;
        }

        _channel = channel;
        _clientService = clientService;
        _blockChainService = blockChainService;
        _clientInfo = await GetInfoAsync(cancellationToken);
        IsRunning = _clientInfo.IsRunning;
        IsAttached = true;
        _logger.LogDebug("Client is attached: {Address}", Address);
        Attached?.Invoke(this, EventArgs.Empty);
    }

    public async Task DetachAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_channel is null)
        {
            throw new InvalidOperationException("Client is not attached.");
        }

        if (_clientService is not null)
        {
            _clientService.Started -= ClientService_Started;
            _clientService.Stopped -= ClientService_Stopped;
            _clientService.Disconnected -= ClientService_Disconnected;
            _clientService.Dispose();
            _clientService = null;
        }

        if (_blockChainService is not null)
        {
            _blockChainService.BlockAppended -= BlockChainService_BlockAppended;
            _blockChainService.Dispose();
            _blockChainService = null;
        }

        await _channel.ShutdownAsync();
        _channel.Dispose();
        _channel = null;
        IsRunning = false;
        IsAttached = false;
        _logger.LogDebug("Client is detached: {Address}", Address);
        Detached?.Invoke(this, EventArgs.Empty);
    }

    public async Task StartAsync(INode node, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        if (IsRunning is true)
        {
            throw new InvalidOperationException("Client is already running.");
        }

        if (_clientService is null)
        {
            throw new InvalidOperationException("Client is not attached.");
        }

        var request = new StartRequest
        {
            NodeEndPoint = EndPointUtility.ToString(node.EndPoint),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _clientService.StartAsync(request, callOptions);
        _clientInfo = response.ClientInfo;
        IsRunning = true;
        _logger.LogDebug("Client is started: {Address}", Address);
        await Task.WhenAll(Contents.Select(item => item.StartAsync(cancellationToken)));
        _logger.LogDebug("Client Contents are started: {Address}", Address);
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        if (IsRunning is false)
        {
            throw new InvalidOperationException("Client is not running.");
        }

        if (_clientService is null)
        {
            throw new InvalidOperationException("Client is not attached.");
        }

        await Task.WhenAll(Contents.Select(item => item.StopAsync(cancellationToken)));
        _logger.LogDebug("Client Contents are stopped: {Address}", Address);

        var request = new StopRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _clientService.StopAsync(request, callOptions);
        _clientInfo = ClientInfo.Empty;
        IsRunning = false;
        _logger.LogDebug("Client is stopped: {Address}", Address);
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed is false)
        {
            await _processCancellationTokenSource.CancelAsync();
            _processCancellationTokenSource.Dispose();
            await TaskUtility.TryWait(_processTask);
            _processTask = Task.CompletedTask;
            _process = null;

            if (_clientService is not null)
            {
                _clientService.Disconnected -= ClientService_Disconnected;
                _clientService.Started -= ClientService_Started;
                _clientService.Stopped -= ClientService_Stopped;
                _clientService.Dispose();
                _clientService = null;
            }

            if (_blockChainService is not null)
            {
                _blockChainService.BlockAppended -= BlockChainService_BlockAppended;
                _blockChainService.Dispose();
                _blockChainService = null;
            }

            IsRunning = false;
            _isDisposed = true;
            _logger.LogDebug("Client is disposed: {Address}", Address);
            Disposed?.Invoke(this, EventArgs.Empty);
            GC.SuppressFinalize(this);
        }
    }

    public ClientProcess CreateProcess() => new(this, _clientOptions);

    public Task StartProcessAsync(AddNewClientOptions options, CancellationToken cancellationToken)
    {
        if (_process is not null)
        {
            throw new InvalidOperationException("Client process is already running.");
        }

        var clientOptions = _clientOptions;
        var process = new ClientProcess(this, clientOptions)
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

    private void ClientService_Started(object? sender, ClientEventArgs e)
    {
        _clientInfo = e.ClientInfo;
        IsRunning = true;
        Started?.Invoke(this, EventArgs.Empty);
    }

    private void ClientService_Stopped(object? sender, EventArgs e)
    {
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    private void BlockChainService_BlockAppended(object? sender, BlockEventArgs e)
    {
        _clientInfo = _clientInfo with { Tip = e.BlockInfo };
        BlockAppended?.Invoke(this, e);
    }

    private void ClientService_Disconnected(object? sender, EventArgs e)
    {
        if (sender is ClientService clientService && _clientService == clientService)
        {
            _clientService.Disconnected -= ClientService_Disconnected;
            _clientService.Started -= ClientService_Started;
            _clientService.Stopped -= ClientService_Stopped;
            _clientService.Dispose();
            _clientService = null;
            if (_blockChainService is not null)
            {
                _blockChainService.BlockAppended -= BlockChainService_BlockAppended;
                _blockChainService.Dispose();
                _blockChainService = null;
            }

            if (_channel is not null)
            {
                _channel.Dispose();
                _channel = null;
            }

            Detached?.Invoke(this, EventArgs.Empty);
        }
    }
}
