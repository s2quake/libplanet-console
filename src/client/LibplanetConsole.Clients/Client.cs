using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Clients.Serializations;
using LibplanetConsole.Clients.Services;
using LibplanetConsole.Common;
using LibplanetConsole.Nodes;
using LibplanetConsole.Nodes.Serializations;

namespace LibplanetConsole.Clients;

[Export]
[Export(typeof(IClient))]
internal sealed class Client : IClient
{
    private readonly PrivateKey _privateKey;
    private readonly RemoteNodeContext _remoteNodeContext;
    private Guid _closeToken;

    [ImportingConstructor]
    public Client(ApplicationOptions options, RemoteNodeContext remoteNodeContext)
    {
        _privateKey = PrivateKeyUtility.Parse(options.PrivateKey);
        _remoteNodeContext = remoteNodeContext;
        _remoteNodeContext.Closed += RemoteNodeContext_Closed;
    }

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public event EventHandler? Started;

    public event EventHandler<StopEventArgs>? Stopped;

    public PrivateKey PrivateKey => _privateKey;

    public PublicKey PublicKey => _privateKey.PublicKey;

    public Address Address => _privateKey.Address;

    public TextWriter Out { get; set; } = Console.Out;

    public ClientInfo Info => new()
    {
        PrivateKey = PrivateKeyUtility.ToString(PrivateKey),
        PublicKey = PublicKeyUtility.ToString(PublicKey),
        Address = AddressUtility.ToString(Address),
    };

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

    public void InvokeBlockAppendedEvent(BlockInfo blockInfo)
    {
        BlockAppended?.Invoke(this, new BlockEventArgs(blockInfo));
    }

    public async ValueTask DisposeAsync()
    {
        await _remoteNodeContext.CloseAsync(_closeToken);
    }

    private void RemoteNodeContext_Closed(object? sender, EventArgs e)
    {
        _closeToken = Guid.Empty;
        ClientOptions = new();
        IsRunning = false;
        Stopped?.Invoke(this, new(StopReason.Disconnected));
    }
}
