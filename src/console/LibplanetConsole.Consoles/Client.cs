using System.Collections;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Security;
using LibplanetConsole.Clients;
using LibplanetConsole.Clients.Services;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Frameworks;
using Serilog;

namespace LibplanetConsole.Consoles;

internal sealed class Client : IClient, IClientCallback
{
    private readonly ApplicationContainer _container;
    private readonly SecureString _privateKey;
    private readonly RemoteServiceContext _remoteServiceContext;
    private readonly RemoteService<IClientService, IClientCallback> _remoteService;
    private readonly IClientContent[] _contents;
    private readonly ILogger _logger;
    private Guid _closeToken;
    private ClientInfo _clientInfo;
    private INode? _node;
    private bool _isDisposed;
    private bool _isInProgress;

    public Client(ApplicationBase application, AppPrivateKey privateKey)
    {
        _container = application.CreateChildContainer(this);
        _privateKey = privateKey.ToSecureString();
        _container.ComposeExportedValue<IClient>(this);
        _contents = [.. _container.GetExportedValues<IClientContent>()];
        _remoteService = new(this);
        _remoteServiceContext = new RemoteServiceContext(
            [_remoteService, .. GetRemoteServices(_container)]);
        _logger = application.GetService<ILogger>();
        PublicKey = privateKey.PublicKey;
        _remoteServiceContext.Closed += RemoteServiceContext_Closed;
        _logger.Debug("Client is created: {Address}", Address);
    }

    public event EventHandler? Attached;

    public event EventHandler? Detached;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public event EventHandler? Disposed;

    public AppPublicKey PublicKey { get; }

    public AppAddress Address => PublicKey.Address;

    public bool IsAttached => _closeToken != Guid.Empty;

    public bool IsRunning { get; private set; }

    public AppEndPoint EndPoint
    {
        get => _remoteServiceContext.EndPoint;
        set => _remoteServiceContext.EndPoint = value;
    }

    public ClientInfo Info => _clientInfo;

    public object? GetService(Type serviceType)
    {
        if (serviceType == typeof(IServiceProvider))
        {
            return this;
        }

        if (serviceType == typeof(INode))
        {
            return _node;
        }

        if (serviceType == typeof(IEnumerable<IClientContent>))
        {
            return _contents;
        }

        if (typeof(IEnumerable).IsAssignableFrom(serviceType) &&
            serviceType.GenericTypeArguments.Length == 1)
        {
            var itemType = serviceType.GenericTypeArguments[0];
            var items = GetInstances(itemType);
            var listGenericType = typeof(List<>);
            var list = listGenericType.MakeGenericType(itemType);
            var ci = list.GetConstructor([typeof(int)])!;
            var instance = (IList)ci.Invoke([items.Count(),]);
            foreach (var item in items)
            {
                instance.Add(item);
            }

            return instance;
        }
        else
        {
            return GetInstance(serviceType);
        }
    }

    public override string ToString() => $"{Address:S}: {EndPoint}";

    public byte[] Sign(object obj) => AppPrivateKey.FromSecureString(_privateKey).Sign(obj);

    public async Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning != true, "Client is not running.");

        _clientInfo = await _remoteService.Service.GetInfoAsync(cancellationToken);
        return _clientInfo;
    }

    public async Task AttachAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: _closeToken != Guid.Empty,
            message: "Client is already attached.");

        using var scope = new ProgressScope(this);
        _closeToken = await _remoteServiceContext.OpenAsync(cancellationToken);
        _clientInfo = await _remoteService.Service.GetInfoAsync(cancellationToken);
        IsRunning = _clientInfo.IsRunning;
        _logger.Debug("Client is attached: {Address}", Address);
        Attached?.Invoke(this, EventArgs.Empty);
    }

    public async Task DetachAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: _closeToken == Guid.Empty,
            message: "Client is not attached.");

        using var scope = new ProgressScope(this);
        await _remoteServiceContext.CloseAsync(_closeToken, cancellationToken);
        _closeToken = Guid.Empty;
        _logger.Debug("Client is detached: {Address}", Address);
        Detached?.Invoke(this, EventArgs.Empty);
    }

    public async Task StartAsync(INode node, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning == true, "Client is already running.");
        InvalidOperationExceptionUtility.ThrowIf(
            condition: _closeToken == Guid.Empty,
            message: "Client is not attached.");

        _clientInfo = await _remoteService.Service.StartAsync(
            node.EndPoint.ToString(), cancellationToken);
        _node = node;
        IsRunning = true;
        _logger.Debug("Client is started: {Address}", Address);
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning != true, "Client is not running.");
        InvalidOperationExceptionUtility.ThrowIf(
            condition: _closeToken == Guid.Empty,
            message: "Client is not attached.");

        await _remoteService.Service.StopAsync(cancellationToken);
        _node = null;
        _closeToken = Guid.Empty;
        _clientInfo = default;
        IsRunning = false;
        _logger.Debug("Client is stopped: {Address}", Address);
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async Task<AppId> SendTransactionAsync(string text, CancellationToken cancellationToken)
    {
        var transactionOptions = new TransactionOptions
        {
            Text = text,
        };
        return await _remoteService.Service.SendTransactionAsync(
            transactionOptions.Sign(this), cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _remoteServiceContext.Closed -= RemoteServiceContext_Closed;
        if (_closeToken != Guid.Empty)
        {
            await _remoteServiceContext.CloseAsync(_closeToken);
            _closeToken = Guid.Empty;
        }

        IsRunning = false;
        await _container.DisposeAsync();
        _isDisposed = true;
        _logger.Debug("Client is disposed: {Address}", Address);
        Disposed?.Invoke(this, EventArgs.Empty);
        GC.SuppressFinalize(this);
    }

    public ClientProcess CreateProcess()
    {
        var endPoint = EndPoint;
        var application = IServiceProviderExtensions.GetService<ApplicationBase>(this);
        return new ClientProcess(this)
        {
            EndPoint = endPoint,
            PrivateKey = _privateKey,
            LogDirectory = application.Info.LogDirectory,
        };
    }

    void IClientCallback.OnStarted(ClientInfo clientInfo)
    {
        if (_isInProgress != true)
        {
            _clientInfo = clientInfo;
            IsRunning = true;
            Started?.Invoke(this, EventArgs.Empty);
        }
    }

    void IClientCallback.OnStopped()
    {
        if (_isInProgress != true)
        {
            _clientInfo = default;
            IsRunning = false;
            Stopped?.Invoke(this, EventArgs.Empty);
        }
    }

    private static IEnumerable<IRemoteService> GetRemoteServices(
        CompositionContainer compositionContainer)
    {
        foreach (var item in compositionContainer.GetExportedValues<IClientContentService>())
        {
            yield return item.RemoteService;
        }
    }

    private object? GetInstance(Type serviceType)
    {
        var contractName = AttributedModelServices.GetContractName(serviceType);
        return _container.GetExportedValue<object>(contractName);
    }

    private IEnumerable<object> GetInstances(Type service)
    {
        var contractName = AttributedModelServices.GetContractName(service);
        return _container.GetExportedValues<object>(contractName);
    }

    private void RemoteServiceContext_Closed(object? sender, EventArgs e)
    {
        if (_isInProgress != true && IsRunning == true)
        {
            _closeToken = Guid.Empty;
            Detached?.Invoke(this, EventArgs.Empty);
        }
    }

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
