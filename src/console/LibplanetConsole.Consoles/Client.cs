using System.Collections;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Net;
using System.Security;
using Libplanet.Crypto;
using LibplanetConsole.Clients;
using LibplanetConsole.Clients.Serializations;
using LibplanetConsole.Clients.Services;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;
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
    private bool _isDisposed;

    public Client(ApplicationBase application, PrivateKey privateKey, EndPoint endPoint)
    {
        _container = application.CreateChildContainer(this);
        _privateKey = PrivateKeyUtility.ToSecureString(privateKey);
        _container.ComposeExportedValue<IClient>(this);
        _contents = [.. _container.GetExportedValues<IClientContent>()];
        _remoteService = new(this);
        _remoteServiceContext = new RemoteServiceContext(
            [_remoteService, .. GetRemoteServices(_container)])
        {
            EndPoint = endPoint,
        };
        _application = application;
        PublicKey = privateKey.PublicKey;
        _remoteServiceContext.Closed += RemoteServiceContext_Closed;
    }

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public event EventHandler? Disposed;

    public PublicKey PublicKey { get; }

    public Address Address => PublicKey.Address;

    public bool IsOnline { get; private set; } = true;

    public bool IsRunning { get; private set; }

    public ClientOptions ClientOptions { get; private set; } = ClientOptions.Default;

    public EndPoint EndPoint => _remoteServiceContext.EndPoint;

    public ClientInfo Info => _clientInfo;

    public object? GetService(Type serviceType)
    {
        if (serviceType == typeof(IServiceProvider))
        {
            return this;
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

    public async Task StartAsync(ClientOptions clientOptions, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning == true, "Client is already running.");

        _closeToken = await _remoteServiceContext.OpenAsync(cancellationToken);
        _clientInfo = await _remoteService.Service.StartAsync(clientOptions, cancellationToken);
        ClientOptions = clientOptions;
        IsRunning = true;
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning != true, "Client is not running.");

        await _remoteService.Service.StopAsync(cancellationToken);
        await _remoteServiceContext.CloseAsync(_closeToken, cancellationToken);
        ClientOptions = ClientOptions.Default;
        IsRunning = false;
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _remoteServiceContext.Closed -= RemoteServiceContext_Closed;
        await _remoteServiceContext.CloseAsync(_closeToken);
        ClientOptions = ClientOptions.Default;
        IsRunning = false;
        await _container.DisposeAsync();
        _isDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
        GC.SuppressFinalize(this);
    }

    void IClientCallback.OnStarted(ClientInfo clientInfo)
    {
        _clientInfo = clientInfo;
    }

    void IClientCallback.OnStopped()
    {
        _clientInfo = new();
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
        ClientOptions = ClientOptions.Default;
        IsRunning = false;
        _isDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
    }
}
