using System.Diagnostics;
using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.Threading;
using LibplanetConsole.Console.Extensions;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Grpc.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Console;

internal sealed class Client : IClient
{
    private readonly IServiceProvider _serviceProvider;
    private readonly PrivateKey _privateKey;
    private readonly ILogger _logger;
    private readonly CriticalSection _criticalSection = new("Process");
    private ClientService? _service;
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
        Options = clientOptions;
        _privateKey = clientOptions.PrivateKey;
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

    public ClientOptions Options { get; private set; }

    public EndPoint EndPoint
    {
        get => Options.EndPoint;
        set
        {
            if (IsAttached is true)
            {
                throw new InvalidOperationException("Client is attached.");
            }

            Options = Options with { EndPoint = value };
        }
    }

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

    public override string ToString() => $"{Address}";

    public byte[] Sign(object obj) => Options.PrivateKey.Sign(obj);

    public async Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_service is null)
        {
            throw new InvalidOperationException("Client is not attached.");
        }

        var request = new GetInfoRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _service.GetInfoAsync(request, callOptions);
        _info = response.ClientInfo;
        return _info;
    }

    public async Task AttachAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        using var scope = _criticalSection.Scope();
        if (IsAttached is true)
        {
            throw new InvalidOperationException("Client is already attached.");
        }

        var channel = ClientChannel.CreateChannel(Options.EndPoint);
        var clientService = await ClientService.CreateAsync(channel, cancellationToken);

        _channel = channel;
        _service = clientService;
        _service.Started += Service_Started;
        _service.Stopped += Service_Stopped;
        _service.Disconnected += Service_Disconnected;
        _info = await GetInfoAsync(cancellationToken);
        IsRunning = _info.IsRunning;
        IsAttached = true;
        _logger.LogDebug("Client is attached: {Address}", Address);
        Attached?.Invoke(this, EventArgs.Empty);
        if (IsRunning is true)
        {
            _logger.LogDebug("Client is started in the Attach: {Address}", Address);
            await Contents.StartAsync(cancellationToken);
            _logger.LogDebug("Client Contents are started in the Attach: {Address}", Address);
            Started?.Invoke(this, EventArgs.Empty);
        }
    }

    public async Task DetachAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        using var scope = _criticalSection.Scope();
        if (_channel is null)
        {
            throw new InvalidOperationException("Client is not attached.");
        }

        if (IsRunning is true)
        {
            await Contents.StopAsync(cancellationToken);
            _logger.LogDebug("Client Contents are stopped in the Attach: {Address}", Address);
            _info = ClientInfo.Empty;
            _logger.LogDebug("Client is stopped in the Attach: {Address}", Address);
            Stopped?.Invoke(this, EventArgs.Empty);
        }

        if (_service is not null)
        {
            _service.Started -= Service_Started;
            _service.Stopped -= Service_Stopped;
            _service.Disconnected -= Service_Disconnected;
            _service.Dispose();
            _service = null;
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
        using var scope = _criticalSection.Scope();
        if (IsRunning is true)
        {
            throw new InvalidOperationException("Client is already running.");
        }

        if (_service is null)
        {
            throw new InvalidOperationException("Client is not attached.");
        }

        var request = new StartRequest
        {
            NodeEndPoint = EndPointUtility.ToString(node.EndPoint),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _service.StartAsync(request, callOptions);
        _info = response.ClientInfo;
        IsRunning = true;
        _logger.LogDebug("Client is started: {Address}", Address);
        await Contents.StartAsync(cancellationToken);
        await Contents.StartAsync(cancellationToken);
        _logger.LogDebug("Client Contents are started: {Address}", Address);
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        using var scope = _criticalSection.Scope();
        if (IsRunning is false)
        {
            throw new InvalidOperationException("Client is not running.");
        }

        if (_service is null)
        {
            throw new InvalidOperationException("Client is not attached.");
        }

        await Contents.StopAsync(cancellationToken);
        _logger.LogDebug("Client Contents are stopped: {Address}", Address);

        var request = new StopRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.StopAsync(request, callOptions);
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

            if (_service is not null)
            {
                _service.Started -= Service_Started;
                _service.Stopped -= Service_Stopped;
                _service.Disconnected -= Service_Disconnected;
                _service.Dispose();
                _service = null;
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

        if (true)
        {
            using var scope = _criticalSection.Scope();
            var process = CreateProcess(options);
            var processCancellationTokenSource = new CancellationTokenSource();
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken, processCancellationTokenSource.Token);

            _logger.LogDebug("Commands: {CommandLine}", process.ToString());
            _processTask = process.RunAsync(cancellationTokenSource.Token)
                .ContinueWith(
                    task =>
                    {
                        if (task.IsFaulted is true)
                        {
                            _logger.LogError(task.Exception, "Failed to run the client process.");
                        }

                        _processTask = Task.CompletedTask;
                        _process = null;
                        _processCancellationTokenSource?.Dispose();
                        _processCancellationTokenSource = null;
                        cancellationTokenSource.Dispose();
                    },
                    cancellationToken);
            _process = process;
            _processCancellationTokenSource = processCancellationTokenSource;
        }

        if (options.Detach is false)
        {
            await AttachAsync(cancellationToken);
        }

        if (IsAttached is true && Options.NodeEndPoint is null)
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

        using var scope = _criticalSection.Scope();
        await TaskUtility.TryWait(_processTask);
        _processTask = Task.CompletedTask;
        _process = null;
    }

    public async Task<TxId> SendTransactionAsync(
        IAction[] actions, CancellationToken cancellationToken)
    {
        var blockChain = _serviceProvider.GetRequiredKeyedService<ClientBlockChain>(IClient.Key);
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

    public string GetCommandLine()
    {
        if (_commandLine is null)
        {
            var process = CreateProcess(ProcessOptions.Default);
            _commandLine = process.GetCommandLine();
        }

        return _commandLine ?? throw new UnreachableException("Process is not created.");
    }

    private async void Service_Started(object? sender, ClientEventArgs e)
    {
        _info = e.ClientInfo;
        IsRunning = true;
        await Contents.StartAsync(default);
        Started?.Invoke(this, EventArgs.Empty);
    }

    private async void Service_Stopped(object? sender, EventArgs e)
    {
        await Contents.StopAsync(default);
        _info = ClientInfo.Empty;
        IsRunning = false;
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    private void Service_Disconnected(object? sender, EventArgs e)
    {
        if (sender is ClientService clientService && _service == clientService)
        {
            _service.Started -= Service_Started;
            _service.Stopped -= Service_Stopped;
            _service.Disconnected -= Service_Disconnected;
            _service.Dispose();
            _service = null;

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
        var clientOptions = Options;
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
