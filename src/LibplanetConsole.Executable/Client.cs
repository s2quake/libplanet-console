using System.Net;
using Bencodex.Types;
using JSSoft.Communication;
using Libplanet.Crypto;
using LibplanetConsole.ClientServices;
using LibplanetConsole.ClientServices.Serializations;

namespace LibplanetConsole.Executable;

internal sealed class Client :
    IClientCallback, IAsyncDisposable, IIdentifier, IClient
{
    private readonly PrivateKey _privateKey;
    private readonly ClientContext _clientContext;
    private readonly ClientService<IClientService, IClientCallback> _clientService;
    private Guid _closeToken;
    private ClientInfo _clientInfo = new();

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

    public event EventHandler? Disposed;

    public PrivateKey PrivateKey => _privateKey;

    public PublicKey PublicKey => _privateKey.PublicKey;

    public Address Address => _privateKey.Address;

    public bool IsOnline { get; private set; } = true;

    public TextWriter Out { get; set; } = Console.Out;

    public bool IsRunning { get; private set; }

    public string Identifier => Address.ToString()[0..8];

    public ClientOptions ClientOptions { get; private set; } = ClientOptions.Default;

    public EndPoint EndPoint => _clientContext.EndPoint;

    PrivateKey IIdentifier.PrivateKey => _privateKey;

    public override string ToString()
    {
        return $"{Address.ToString()[0..8]}: {EndPointUtility.ToString(EndPoint)}";
    }

    public async Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        _clientInfo = await _clientService.Server.GetInfoAsync(cancellationToken);
        return _clientInfo;
    }

    public async Task StartAsync(ClientOptions clientOptions, CancellationToken cancellationToken)
    {
        _closeToken = await _clientContext.OpenAsync(cancellationToken);
        _clientInfo = await _clientService.Server.StartAsync(clientOptions, cancellationToken);
        ClientOptions = clientOptions;
        IsRunning = true;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ClientOptions = ClientOptions.Default;
        IsRunning = false;
        await _clientService.Server.StopAsync(cancellationToken);
        await _clientContext.CloseAsync(_closeToken, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public object? GetService(Type serviceType)
    {
        return null;
    }

    private void ClientContext_Disconnected(object? sender, EventArgs e)
    {
        Disposed?.Invoke(this, EventArgs.Empty);
    }

    private void ClientContext_Faulted(object? sender, EventArgs e)
    {
        Disposed?.Invoke(this, EventArgs.Empty);
    }
}
