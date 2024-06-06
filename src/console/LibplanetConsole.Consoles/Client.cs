using System.Collections;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Net;
using System.Security;
using Libplanet.Crypto;
using Libplanet.Types.Tx;
using LibplanetConsole.Clients.Serializations;
using LibplanetConsole.Clients.Services;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles;

internal sealed class Client :
    IClientCallback, IAsyncDisposable, IAddressable, IClient
{
    private readonly ApplicationContainer _container;
    private readonly SecureString _privateKey;
    private readonly RemoteServiceContext _remoteServiceContext;
    private readonly RemoteService<IClientService, IClientCallback> _remoteService;
    private readonly IClientContent[] _contents;
    private readonly ApplicationBase _application;
    private Guid _closeToken;
    private ClientInfo _clientInfo = new();
    private INode? _node;
    private bool _isDisposed;
    private bool _isInProgress;

    public Client(ApplicationBase application, PrivateKey privateKey)
    {
        _container = application.CreateChildContainer(this);
        _privateKey = PrivateKeyUtility.ToSecureString(privateKey);
        _container.ComposeExportedValue<IClient>(this);
        _contents = [.. _container.GetExportedValues<IClientContent>()];
        _remoteService = new(this);
        _remoteServiceContext = new RemoteServiceContext(
            [_remoteService, .. GetRemoteServices(_container)]);
        _application = application;
        PublicKey = privateKey.PublicKey;
        _remoteServiceContext.Closed += RemoteServiceContext_Closed;
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

    public EndPoint EndPoint
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

        if (typeof(IEnumerable).IsAssignableFrom(serviceType) &&
            serviceType.GenericTypeArguments.Length == 1)
        {
            var itemType = serviceType.GenericTypeArguments.First();
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

    public override string ToString()
    {
        return $"{(ShortAddress)Address}: {EndPointUtility.ToString(EndPoint)}";
    }

    public byte[] Sign(object obj)
    {
        var privateKey = PrivateKeyUtility.FromSecureString(_privateKey);
        return PrivateKeyUtility.Sign(privateKey, obj);
    }

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
            EndPointUtility.ToString(node.EndPoint), cancellationToken);
        _node = node;
        IsRunning = true;
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
        _clientInfo = new();
        IsRunning = false;
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async Task<TxId> SendTransactionAsync(string text, CancellationToken cancellationToken)
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
        Disposed?.Invoke(this, EventArgs.Empty);
        GC.SuppressFinalize(this);
    }

    public ClientProcess CreateClientProcess()
    {
        var endPoint = EndPoint;
        var application = IServiceProviderExtensions.GetService<ApplicationBase>(this);
        return new ClientProcess
        {
            EndPoint = endPoint,
            PrivateKey = _privateKey,
            NewTerminal = application.Info.NewTerminal,
            ManualStart = application.Info.ManualStart,
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
            _clientInfo = new();
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
