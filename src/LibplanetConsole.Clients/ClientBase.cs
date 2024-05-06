using JSSoft.Communication.Extensions;
using Libplanet.Crypto;
using LibplanetConsole.Clients.Serializations;
using LibplanetConsole.Common;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.Clients;

public abstract class ClientBase : IAsyncDisposable
{
    private readonly PrivateKey _privateKey;
    private readonly IRemoteNodeContext _remoteNodeContext;
    private Guid _closeToken;

    public ClientBase(PrivateKey privateKey, IRemoteNodeContext remoteNodeContext)
    {
        _privateKey = privateKey;
        _remoteNodeContext = remoteNodeContext;
        _remoteNodeContext.Disconnected += RemoteNodeContext_Disconnected;
        _remoteNodeContext.Faulted += RemoteNodeContext_Faulted;
    }

    public event EventHandler? Started;

    public event EventHandler<StopEventArgs>? Stopped;

    public PrivateKey PrivateKey => _privateKey;

    public PublicKey PublicKey => _privateKey.PublicKey;

    public Address Address => _privateKey.Address;

    public TextWriter Out { get; set; } = Console.Out;

    public ClientInfo Info => new(this);

    public bool IsRunning { get; private set; }

    public ClientOptions ClientOptions { get; private set; } = new();

    public override string ToString() => $"[{Address}]";

    public async Task StartAsync(ClientOptions clientOptions, CancellationToken cancellationToken)
    {
        _remoteNodeContext.EndPoint = clientOptions.NodeEndPoint;
        _closeToken = await _remoteNodeContext.OpenAsync(cancellationToken);
        ClientOptions = clientOptions;
        IsRunning = true;
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _remoteNodeContext.CloseAsync(_closeToken, cancellationToken);
        _closeToken = Guid.Empty;
        ClientOptions = new();
        IsRunning = false;
        Stopped?.Invoke(this, new(StopReason.None));
    }

    public async ValueTask DisposeAsync()
    {
        await _remoteNodeContext.ReleaseAsync(_closeToken);
        GC.SuppressFinalize(this);
    }

    private void RemoteNodeContext_Disconnected(object? sender, EventArgs e)
    {
        _closeToken = Guid.Empty;
        ClientOptions = new();
        IsRunning = false;
        Stopped?.Invoke(this, new(StopReason.Disconnected));
    }

    private async void RemoteNodeContext_Faulted(object? sender, EventArgs e)
    {
        _closeToken = Guid.Empty;
        ClientOptions = new();
        IsRunning = false;
        await _remoteNodeContext.AbortAsync();
        Stopped?.Invoke(this, new(StopReason.Faulted));
    }
}
