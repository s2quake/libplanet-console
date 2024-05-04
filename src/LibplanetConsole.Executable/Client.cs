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
    IClientCallback, IAsyncDisposable, IIdentifier, IClient
{
    private readonly PrivateKey _privateKey;
    private readonly ClientContext _clientContext;
    private readonly ClientService<IClientService, IClientCallback> _clientService;
    private Guid _closeToken;
    private ClientInfo _clientInfo = new();
    private bool _isDisposed;

    public Client(PrivateKey privateKey, EndPoint endPoint)
    {
        _privateKey = privateKey;
        _clientService = new(this);
        _clientContext = new ClientContext(
            _clientService)
        {
            EndPoint = endPoint,
        };
        _clientContext.Disconnected += ClientContext_Disconnected;
        _clientContext.Faulted += ClientContext_Faulted;
    }

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public event EventHandler? Disposed;

    public PrivateKey PrivateKey => _privateKey;

    public PublicKey PublicKey => _privateKey.PublicKey;

    public Address Address => _privateKey.Address;

    public bool IsOnline { get; private set; } = true;

    public TextWriter Out { get; set; } = Console.Out;

    public bool IsRunning { get; private set; }

    public string Identifier => (ShortAddress)Address;

    public ClientOptions ClientOptions { get; private set; } = ClientOptions.Default;

    public EndPoint EndPoint => _clientContext.EndPoint;

    PrivateKey IIdentifier.PrivateKey => _privateKey;

    public override string ToString()
    {
        return $"{Identifier}: {EndPointUtility.ToString(EndPoint)}";
    }

    public async Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning != true, "Client is not running.");

        _clientInfo = await _clientService.Server.GetInfoAsync(cancellationToken);
        return _clientInfo;
    }

    public async Task StartAsync(ClientOptions clientOptions, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning == true, "Client is already running.");

        _closeToken = await _clientContext.OpenAsync(cancellationToken);
        _clientInfo = await _clientService.Server.StartAsync(clientOptions, cancellationToken);
        ClientOptions = clientOptions;
        IsRunning = true;
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning != true, "Client is not running.");

        await _clientService.Server.StopAsync(cancellationToken);
        await _clientContext.CloseAsync(_closeToken, cancellationToken);
        ClientOptions = ClientOptions.Default;
        IsRunning = false;
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _clientContext.Disconnected -= ClientContext_Disconnected;
        _clientContext.Faulted -= ClientContext_Faulted;
        await _clientContext.ReleaseAsync(_closeToken);
        ClientOptions = ClientOptions.Default;
        IsRunning = false;
        _isDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
        GC.SuppressFinalize(this);
    }

    private void ClientContext_Disconnected(object? sender, EventArgs e)
    {
        ClientOptions = ClientOptions.Default;
        IsRunning = false;
        _isDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
    }

    private void ClientContext_Faulted(object? sender, EventArgs e)
    {
        ClientOptions = ClientOptions.Default;
        IsRunning = false;
        _isDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
    }
}
