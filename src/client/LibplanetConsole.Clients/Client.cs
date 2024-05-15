using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Clients.Serializations;
using LibplanetConsole.Clients.Services;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Nodes;
using LibplanetConsole.Nodes.Serializations;

namespace LibplanetConsole.Clients;

[Export]
[Export(typeof(IClient))]
[method: ImportingConstructor]
internal sealed class Client(IApplication application, ApplicationOptions options)
    : IClient
{
    private readonly IApplication _application = application;
    private readonly PrivateKey _privateKey = PrivateKeyUtility.Parse(options.PrivateKey);
    private RemoteNodeContext? _remoteNodeContext;
    private Guid _closeToken;

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

    private RemoteNodeContext RemoteNodeContext
    {
        get
        {
            if (_remoteNodeContext is null)
            {
                _remoteNodeContext = _application.GetService<RemoteNodeContext>();
                _remoteNodeContext.Closed += RemoteNodeContext_Closed;
            }

            return _remoteNodeContext;
        }
    }

    public override string ToString() => $"[{Address}]";

    public async Task StartAsync(ClientOptions clientOptions, CancellationToken cancellationToken)
    {
        RemoteNodeContext.EndPoint = clientOptions.NodeEndPoint;
        _closeToken = await RemoteNodeContext.OpenAsync(cancellationToken);
        ClientOptions = clientOptions;
        IsRunning = true;
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await RemoteNodeContext.CloseAsync(_closeToken, cancellationToken);
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
        await RemoteNodeContext.CloseAsync(_closeToken);
    }

    private void RemoteNodeContext_Closed(object? sender, EventArgs e)
    {
        _closeToken = Guid.Empty;
        ClientOptions = new();
        IsRunning = false;
        Stopped?.Invoke(this, new(StopReason.Disconnected));
    }
}
