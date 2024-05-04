using JSSoft.Communication.Extensions;
using Libplanet.Crypto;
using LibplanetConsole.ClientServices.Serializations;
using LibplanetConsole.Common;
using LibplanetConsole.NodeServices;
using LibplanetConsole.NodeServices.Serializations;
using NodeRemoteContext = JSSoft.Communication.ClientContext;

namespace LibplanetConsole.ClientServices;

public abstract class ClientBase
    : IAsyncDisposable, INodeCallback
{
    private readonly PrivateKey _privateKey;
    private readonly NodeRemoteContext _nodeRemoteContext;
    private Guid _closeToken;

    public ClientBase(PrivateKey privateKey, IRemoteService[] remoteServices)
    {
        _privateKey = privateKey;
        _nodeRemoteContext = new NodeRemoteContext(
            [new NodeRemoteService(this), .. remoteServices]
        );
        _nodeRemoteContext.Disconnected += NodeRemoteContext_Disconnected;
        _nodeRemoteContext.Faulted += NodeRemoteContext_Faulted;
    }

    public event EventHandler<BlockEventArgs>? BlockAppended;

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
        _nodeRemoteContext.EndPoint = clientOptions.NodeEndPoint;
        _closeToken = await _nodeRemoteContext.OpenAsync(cancellationToken);
        ClientOptions = clientOptions;
        IsRunning = true;
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _nodeRemoteContext.CloseAsync(_closeToken, cancellationToken);
        _closeToken = Guid.Empty;
        ClientOptions = new();
        IsRunning = false;
        Stopped?.Invoke(this, new(StopReason.None));
    }

    public async ValueTask DisposeAsync()
    {
        await _nodeRemoteContext.ReleaseAsync(_closeToken);
        GC.SuppressFinalize(this);
    }

    void INodeCallback.OnBlockAppended(BlockInfo blockInfo)
    {
        BlockAppended?.Invoke(this, new BlockEventArgs(blockInfo));
    }

    private void NodeRemoteContext_Disconnected(object? sender, EventArgs e)
    {
        _closeToken = Guid.Empty;
        ClientOptions = new();
        IsRunning = false;
        Stopped?.Invoke(this, new(StopReason.Disconnected));
    }

    private async void NodeRemoteContext_Faulted(object? sender, EventArgs e)
    {
        _closeToken = Guid.Empty;
        ClientOptions = new();
        IsRunning = false;
        await _nodeRemoteContext.AbortAsync();
        Stopped?.Invoke(this, new(StopReason.Faulted));
    }
}
