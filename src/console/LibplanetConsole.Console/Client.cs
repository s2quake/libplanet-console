using System.Diagnostics;
using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.Threading;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Grpc.Blockchain;
using LibplanetConsole.Grpc.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Console;

internal sealed partial class Client : IClient
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ClientOptions _clientOptions;
    private readonly PrivateKey _privateKey;
    private readonly ILogger _logger;
    private ClientService? _clientService;
    private BlockChainService? _blockChainService;
    private GrpcChannel? _channel;
    private ClientInfo _info;
    private bool _isDisposed;
    private ClientProcess? _process;
    private CancellationTokenSource? _processCancellationTokenSource;
    private Task _processTask = Task.CompletedTask;
    private IClientContent[]? _contents;
    private string? _commandLine;

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

    public int ProcessId => _process?.Id ?? -1;

    public EndPoint EndPoint => _clientOptions.EndPoint;

    public ClientInfo Info => _info;

    public IClientContent[] Contents
    {
        get => _contents ?? throw new InvalidOperationException("Contents is not initialized.");
        set => _contents = value;
    }

    public object? GetService(Type serviceType) => _serviceProvider.GetService(serviceType);

    public object? GetKeyedService(Type serviceType, object? serviceKey)
    {
        if (_serviceProvider is IKeyedServiceProvider serviceProvider)
        {
            return serviceProvider.GetKeyedService(serviceType, serviceKey);
        }

        throw new InvalidOperationException("Service provider does not support keyed service.");
    }

    public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
    {
        if (_serviceProvider is IKeyedServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredKeyedService(serviceType, serviceKey);
        }

        throw new InvalidOperationException("Service provider does not support keyed service.");
    }

    public override string ToString() => $"{Address}: {EndPointUtility.ToString(EndPoint)}";

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
        _info = response.ClientInfo;
        return _info;
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
        clientService.Disconnected += ClientService_Disconnected;
        blockChainService.BlockAppended += BlockChainService_BlockAppended;
        try
        {
            await clientService.InitializeAsync(cancellationToken);
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
        _info = await GetInfoAsync(cancellationToken);
        IsRunning = _info.IsRunning;
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
        _info = response.ClientInfo;
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
        _info = ClientInfo.Empty;
        IsRunning = false;
        _logger.LogDebug("Client is stopped: {Address}", Address);
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed is false)
        {
            if (_processCancellationTokenSource is not null)
            {
                await _processCancellationTokenSource.CancelAsync();
                _processCancellationTokenSource.Dispose();
                _processCancellationTokenSource = null;
            }

            await TaskUtility.TryWait(_processTask);
            _processTask = Task.CompletedTask;
            _process = null;

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

            IsRunning = false;
            _isDisposed = true;
            _logger.LogDebug("Client is disposed: {Address}", Address);
            Disposed?.Invoke(this, EventArgs.Empty);
            GC.SuppressFinalize(this);
        }
    }

    public async Task StartProcessAsync(ProcessOptions options, CancellationToken cancellationToken)
    {
        if (_process is not null)
        {
            throw new InvalidOperationException("Client process is already running.");
        }

        var process = CreateProcess(options);
        var processCancellationTokenSource = new CancellationTokenSource();
        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken, processCancellationTokenSource.Token);
        _logger.LogDebug("Commands: {CommandLine}", process.ToString());
        _processTask = process.RunAsync(cancellationTokenSource.Token)
            .ContinueWith(
                task =>
                {
                    _processTask = Task.CompletedTask;
                    _process = null;
                    _processCancellationTokenSource?.Dispose();
                    _processCancellationTokenSource = null;
                    cancellationTokenSource.Dispose();
                },
                cancellationToken);
        _process = process;
        _processCancellationTokenSource = processCancellationTokenSource;

        if (options.Detach is false)
        {
            await AttachAsync(cancellationToken);
        }

        if (IsAttached is true && _clientOptions.NodeEndPoint is null)
        {
            var nodes = _serviceProvider.GetRequiredService<NodeCollection>();
            var node = nodes.RandomNode();
            await StartAsync(node, cancellationToken);
        }
    }

    public async Task StopProcessAsync(CancellationToken cancellationToken)
    {
        if (_process is null)
        {
            throw new InvalidOperationException("Node process is not running.");
        }

        if (_processCancellationTokenSource is not null)
        {
            await _processCancellationTokenSource.CancelAsync();
            _processCancellationTokenSource.Dispose();
            _processCancellationTokenSource = null;
        }

        await TaskUtility.TryWait(_processTask);
        _processTask = Task.CompletedTask;
        _process = null;
    }

    public string GetCommandLine()
    {
        if (_commandLine is null)
        {
            var process = CreateProcess(ProcessOptions.Default);
            _commandLine = process.GetCommandLine();
        }

        return _commandLine ?? throw new UnreachableException("Process is not created.");
    }

    private void ClientService_Started(object? sender, ClientEventArgs e)
    {
        _info = e.ClientInfo;
        IsRunning = true;
        Started?.Invoke(this, EventArgs.Empty);
    }

    private void ClientService_Stopped(object? sender, EventArgs e)
    {
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    private void BlockChainService_BlockAppended(object? sender, BlockEventArgs e)
    {
        _info = _info with { Tip = e.BlockInfo };
        BlockAppended?.Invoke(this, e);
    }

    private void ClientService_Disconnected(object? sender, EventArgs e)
    {
        if (sender is ClientService clientService && _clientService == clientService)
        {
            _clientService.Started -= ClientService_Started;
            _clientService.Stopped -= ClientService_Stopped;
            _clientService.Disconnected -= ClientService_Disconnected;
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

            if (IsRunning is true)
            {
                IsRunning = false;
                Stopped?.Invoke(this, EventArgs.Empty);
            }

            IsAttached = false;
            Detached?.Invoke(this, EventArgs.Empty);
        }
    }

    private ClientProcess CreateProcess(ProcessOptions options)
    {
        var clientOptions = _clientOptions;
        var process = new ClientProcess(clientOptions)
        {
            Detach = options.Detach,
            NewWindow = options.NewWindow,
        };

        if (clientOptions.RepositoryPath == string.Empty)
        {
            var applicationOptions = _serviceProvider.GetRequiredService<IApplicationOptions>();
            if (applicationOptions.LogPath != string.Empty)
            {
                var clientLogPath = Path.Combine(
                    applicationOptions.LogPath, "clients", Address.ToString());
                process.ExtendedArguments.Add("--log-path");
                process.ExtendedArguments.Add(clientLogPath);
            }
        }

        return process;
    }
}
