using JSSoft.Communication;
using JSSoft.Communication.Extensions;
using Libplanet.Crypto;
using LibplanetConsole.ClientServices.Serializations;
using LibplanetConsole.Common;
using LibplanetConsole.NodeServices;
using LibplanetConsole.NodeServices.Serializations;

namespace LibplanetConsole.ClientServices;

public abstract class ClientBase
    : IAsyncDisposable, INodeCallback
{
    private readonly PrivateKey _privateKey;
    private readonly ClientContext _clientContext;
    private readonly ClientService<INodeService, INodeCallback> _nodeService;
    private Guid _closeToken;

    public ClientBase(PrivateKey privateKey)
    {
        _privateKey = privateKey;
        _nodeService = new(this);
        _clientContext = new ClientContext(
            _nodeService);
        _clientContext.Disconnected += ClientContext_Disconnected;
        _clientContext.Faulted += ClientContext_Faulted;
    }

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public event EventHandler? Started;

    public event EventHandler<StopEventArgs>? Stopped;

    public PrivateKey PrivateKey => _privateKey;

    public PublicKey PublicKey => _privateKey.PublicKey;

    public Address Address => _privateKey.Address;

    public TextWriter Out { get; set; } = Console.Out;

    public string Identifier { get; internal set; } = string.Empty;

    public ClientInfo Info => new(this);

    public bool IsRunning { get; private set; }

    public ClientOptions ClientOptions { get; private set; } = new();

    public override string ToString() => $"[{Address}]";

    public async Task StartAsync(ClientOptions clientOptions, CancellationToken cancellationToken)
    {
        _clientContext.EndPoint = clientOptions.NodeEndPoint;
        _closeToken = await _clientContext.OpenAsync(cancellationToken);
        ClientOptions = clientOptions;
        IsRunning = true;
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _clientContext.CloseAsync(_closeToken, cancellationToken);
        _closeToken = Guid.Empty;
        ClientOptions = new();
        IsRunning = false;
        Stopped?.Invoke(this, new(StopReason.None));
    }

    public async ValueTask DisposeAsync()
    {
        await _clientContext.ReleaseAsync(_closeToken);
        GC.SuppressFinalize(this);
    }

    void INodeCallback.OnBlockAppended(BlockInfo blockInfo)
    {
        BlockAppended?.Invoke(this, new BlockEventArgs(blockInfo));
    }

    private void ClientContext_Disconnected(object? sender, EventArgs e)
    {
        _closeToken = Guid.Empty;
        ClientOptions = new();
        IsRunning = false;
        Stopped?.Invoke(this, new(StopReason.Disconnected));
    }

    private async void ClientContext_Faulted(object? sender, EventArgs e)
    {
        _closeToken = Guid.Empty;
        ClientOptions = new();
        IsRunning = false;
        await _clientContext.AbortAsync();
        Stopped?.Invoke(this, new(StopReason.Faulted));
    }
}
