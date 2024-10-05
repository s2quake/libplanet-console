using LibplanetConsole.Client;
using LibplanetConsole.Client.Services;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Console;

internal sealed class Client : IClient, IClientCallback
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ClientOptions _clientOptions;
    // private readonly RemoteService<IClientService, IClientCallback> _remoteService;
    private readonly ILogger _logger;
    private readonly CancellationTokenSource _processCancellationTokenSource = new();
    // private RemoteServiceContext? _remoteServiceContext;
    private Guid _closeToken;
    private ClientInfo _clientInfo;
    private bool _isDisposed;
    private bool _isInProgress;
    private ClientProcess? _process;
    private Task _processTask = Task.CompletedTask;

    public Client(IServiceProvider serviceProvider, ClientOptions clientOptions)
    {
        _serviceProvider = serviceProvider;
        _clientOptions = clientOptions;
        // _remoteService = new(this);
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

    public bool IsAttached => _closeToken != Guid.Empty;

    public bool IsRunning { get; private set; }

    public EndPoint EndPoint => _clientOptions.EndPoint;

    public ClientInfo Info => _clientInfo;

    public object? GetService(Type serviceType) => _serviceProvider.GetService(serviceType);

    public override string ToString() => $"{Address.ToShortString()}: {EndPoint}";

    public byte[] Sign(object obj) => _clientOptions.PrivateKey.Sign(obj);

    public async Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning is not true, "Client is not running.");

        // _clientInfo = await _remoteService.Service.GetInfoAsync(cancellationToken);
        // return _clientInfo;
        throw new NotImplementedException();
    }

    public async Task AttachAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: _closeToken != Guid.Empty,
            message: "Client is already attached.");

        // if (_remoteServiceContext is not null)
        // {
        //     throw new InvalidOperationException("Client is already attached.");
        // }

        using var scope = new ProgressScope(this);
        // _remoteServiceContext = new RemoteServiceContext(
        //     [_remoteService, .. GetRemoteServices(_serviceProvider)])
        // {
        //     EndPoint = _clientOptions.EndPoint,
        // };
        // _closeToken = await _remoteServiceContext.OpenAsync(cancellationToken);
        // _remoteServiceContext.Closed += RemoteServiceContext_Closed;
        // _clientInfo = await _remoteService.Service.GetInfoAsync(cancellationToken);
        IsRunning = _clientInfo.IsRunning;
        _logger.LogDebug("Client is attached: {Address}", Address);
        Attached?.Invoke(this, EventArgs.Empty);
    }

    public async Task DetachAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: _closeToken == Guid.Empty,
            message: "Client is not attached.");

        // if (_remoteServiceContext is null)
        // {
        //     throw new InvalidOperationException("Client is not attached.");
        // }

        // using var scope = new ProgressScope(this);
        // _remoteServiceContext.Closed -= RemoteServiceContext_Closed;
        // await _remoteServiceContext.CloseAsync(_closeToken, cancellationToken);
        _closeToken = Guid.Empty;
        _logger.LogDebug("Client is detached: {Address}", Address);
        Detached?.Invoke(this, EventArgs.Empty);
    }

    public async Task StartAsync(INode node, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning is true, "Client is already running.");
        InvalidOperationExceptionUtility.ThrowIf(
            condition: _closeToken == Guid.Empty,
            message: "Client is not attached.");

        // _clientInfo = await _remoteService.Service.StartAsync(
        //     EndPointUtility.ToString(node.EndPoint), cancellationToken);
        IsRunning = true;
        _logger.LogDebug("Client is started: {Address}", Address);
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning is not true, "Client is not running.");
        InvalidOperationExceptionUtility.ThrowIf(
            condition: _closeToken == Guid.Empty,
            message: "Client is not attached.");

        // await _remoteService.Service.StopAsync(cancellationToken);
        _closeToken = Guid.Empty;
        _clientInfo = default;
        IsRunning = false;
        _logger.LogDebug("Client is stopped: {Address}", Address);
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async Task<TxId> SendTransactionAsync(string text, CancellationToken cancellationToken)
    {
        var transactionOptions = new TransactionOptions
        {
            Text = text,
        };
        // return await _remoteService.Service.SendTransactionAsync(
        //     transactionOptions.Sign(this), cancellationToken);
        throw new NotImplementedException();
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

    void IClientCallback.OnStarted(ClientInfo clientInfo)
    {
        if (_isInProgress is not true)
        {
            _clientInfo = clientInfo;
            IsRunning = true;
            Started?.Invoke(this, EventArgs.Empty);
        }
    }

    void IClientCallback.OnStopped()
    {
        if (_isInProgress is not true)
        {
            _clientInfo = default;
            IsRunning = false;
            Stopped?.Invoke(this, EventArgs.Empty);
        }
    }

    // private static IEnumerable<IRemoteService> GetRemoteServices(
    //     IServiceProvider serviceProvider)
    // {
    //     foreach (var item in serviceProvider.GetServices<IClientContentService>())
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
    //         if (_isInProgress is not true && IsRunning is true)
    //         {
    //             _closeToken = Guid.Empty;
    //             Detached?.Invoke(this, EventArgs.Empty);
    //         }
    //     }
    // }

    private sealed class ProgressScope : IDisposable
    {
        private readonly Client _client;

        public ProgressScope(Client client)
        {
            _client = client;
            _client._isInProgress = true;
        }

        public void Dispose()
        {
            _client._isInProgress = false;
        }
    }
}
