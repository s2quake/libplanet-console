using System.Collections;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Net;
using JSSoft.Communication;
using JSSoft.Communication.Extensions;
using Libplanet.Crypto;
using LibplanetConsole.ClientServices;
using LibplanetConsole.ClientServices.Serializations;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;

namespace LibplanetConsole.Executable;

internal sealed class Client :
    IClientCallback, IAsyncDisposable, IAddressable, IClient
{
    private readonly CompositionContainer _container;
    private readonly PrivateKey _privateKey;
    private readonly RemoteContext _remoteContext;
    private readonly RemoteService<IClientService, IClientCallback> _remoteService;
    private readonly IClientContent[] _contents;
    private Guid _closeToken;
    private ClientInfo _clientInfo = new();
    private bool _isDisposed;

    public Client(CompositionContainer container, PrivateKey privateKey, EndPoint endPoint)
    {
        _container = container;
        _privateKey = privateKey;
        _remoteService = new(this);
        _remoteContext = new RemoteContext(
            _remoteService)
        {
            EndPoint = endPoint,
        };
        _container.ComposeExportedValue<IClient>(this);
        _contents = [.. _container.GetExportedValues<IClientContent>()];
        _remoteContext.Disconnected += RemoteContext_Disconnected;
        _remoteContext.Faulted += RemoteContext_Faulted;
    }

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public event EventHandler? Disposed;

    public PrivateKey PrivateKey => _privateKey;

    public PublicKey PublicKey => _privateKey.PublicKey;

    public Address Address => _privateKey.Address;

    public bool IsOnline { get; private set; } = true;

    public bool IsRunning { get; private set; }

    public ClientOptions ClientOptions { get; private set; } = ClientOptions.Default;

    public EndPoint EndPoint => _remoteContext.EndPoint;

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

    public async Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning != true, "Client is not running.");

        _clientInfo = await _remoteService.Server.GetInfoAsync(cancellationToken);
        return _clientInfo;
    }

    public async Task StartAsync(ClientOptions clientOptions, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning == true, "Client is already running.");

        _closeToken = await _remoteContext.OpenAsync(cancellationToken);
        _clientInfo = await _remoteService.Server.StartAsync(clientOptions, cancellationToken);
        ClientOptions = clientOptions;
        IsRunning = true;
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning != true, "Client is not running.");

        await _remoteService.Server.StopAsync(cancellationToken);
        await _remoteContext.CloseAsync(_closeToken, cancellationToken);
        ClientOptions = ClientOptions.Default;
        IsRunning = false;
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _remoteContext.Disconnected -= RemoteContext_Disconnected;
        _remoteContext.Faulted -= RemoteContext_Faulted;
        await _remoteContext.ReleaseAsync(_closeToken);
        ClientOptions = ClientOptions.Default;
        IsRunning = false;
        _container.Dispose();
        _isDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
        GC.SuppressFinalize(this);
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

    private void RemoteContext_Disconnected(object? sender, EventArgs e)
    {
        ClientOptions = ClientOptions.Default;
        IsRunning = false;
        _isDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
    }

    private void RemoteContext_Faulted(object? sender, EventArgs e)
    {
        ClientOptions = ClientOptions.Default;
        IsRunning = false;
        _isDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
    }
}
