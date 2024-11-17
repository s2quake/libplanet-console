using Libplanet.Net.Messages;
using Libplanet.Net.Options;
using Libplanet.Net.Transports;
using Serilog;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Seed;

public sealed class SeedNode(string name, SeedOptions seedOptions)
{
    private readonly ILogger _logger = Log.ForContext<SeedNode>();

    private ITransport? _transport;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task _task = Task.CompletedTask;
    private Task _refreshTask = Task.CompletedTask;

    public string Name { get; } = name;

    public ILogger Logger => _logger;

    public bool IsRunning { get; private set; }

    public PeerCollection Peers { get; } = new(name, seedOptions);

    public BoundPeer BoundPeer { get; } = new(
        seedOptions.PrivateKey.PublicKey, GetLocalHost(seedOptions.Port));

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (IsRunning)
        {
            throw new InvalidOperationException("Seed node is already running.");
        }

        _cancellationTokenSource
            = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _transport = await CreateTransport(seedOptions);
        _transport.ProcessMessageHandler.Register(ReceiveMessageAsync);
        _task = _transport.StartAsync(_cancellationTokenSource.Token);
        await _transport.WaitForRunningAsync();
        _refreshTask = RefreshContinuouslyAsync(_transport, _cancellationTokenSource.Token);
        IsRunning = true;
        _logger.Information("[{Name}] SeedNode is started: {BoundPeer}", Name, BoundPeer);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!IsRunning)
        {
            throw new InvalidOperationException("Seed node is not running.");
        }

        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        if (_transport is not null)
        {
            await _transport.StopAsync(TimeSpan.FromSeconds(0), cancellationToken);
            _transport.Dispose();
            _transport = null;
        }

        await _refreshTask;
        await _task;
        _refreshTask = Task.CompletedTask;
        _task = Task.CompletedTask;
        IsRunning = false;
        _logger.Information("[{Name}] SeedNode is stopped", Name);
    }

    public async ValueTask DisposeAsync()
    {
        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        if (_transport is not null)
        {
            _transport.Dispose();
            _transport = null;
        }

        await _refreshTask;
        await _task;
        _refreshTask = Task.CompletedTask;
        _task = Task.CompletedTask;
    }

    private static async Task<NetMQTransport> CreateTransport(SeedOptions seedOptions)
    {
        var privateKey = seedOptions.PrivateKey;
        var appProtocolVersion = seedOptions.AppProtocolVersion;
        var appProtocolVersionOptions = new AppProtocolVersionOptions
        {
            AppProtocolVersion = appProtocolVersion,
            TrustedAppProtocolVersionSigners = [],
        };
        var port = seedOptions.Port;
        var hostOptions = new HostOptions("localhost", [], port);
        return await NetMQTransport.Create(privateKey, appProtocolVersionOptions, hostOptions);
    }

    private async Task RefreshContinuouslyAsync(
        ITransport transport, CancellationToken cancellationToken)
    {
        var interval = seedOptions.RefreshInterval;
        var peers = Peers;
        while (cancellationToken.IsCancellationRequested != true)
        {
            try
            {
                await Task.Delay(interval, cancellationToken);
                await peers.RefreshAsync(transport, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private async Task ReceiveMessageAsync(Message message)
    {
        if (_transport is null || _cancellationTokenSource is null)
        {
            throw new InvalidOperationException("Seed node is not running.");
        }

        var messageIdentity = message.Identity;
        var cancellationToken = _cancellationTokenSource.Token;
        var transport = _transport;
        var peers = Peers;
        var id = messageIdentity is not null ? new Guid(messageIdentity) : Guid.Empty;
        var boundPeer = message.Remote;

        peers.AddOrUpdate(boundPeer);

        switch (message.Content)
        {
            case FindNeighborsMsg:
                var alivePeers = peers.GetAlivePeers();
                var neighborsMsg = new NeighborsMsg(alivePeers);
                await transport.ReplyMessageAsync(neighborsMsg, messageIdentity, cancellationToken);
                _logger.Debug(
                    "[{Name}] Response {Msg}: {Peer} {Id} {AlivePeers} ",
                    Name,
                    nameof(NeighborsMsg),
                    boundPeer,
                    id,
                    alivePeers.Length);
                break;

            default:
                var pongMsg = new PongMsg();
                await transport.ReplyMessageAsync(pongMsg, messageIdentity, cancellationToken);
                _logger.Debug(
                    "[{Name}] Response {Msg}: {Peer} {Id}",
                    Name,
                    nameof(PongMsg),
                    boundPeer,
                    id);
                break;
        }
    }
}
