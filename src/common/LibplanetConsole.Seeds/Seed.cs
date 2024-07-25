using Dasync.Collections;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Net.Messages;
using Libplanet.Net.Options;
using Libplanet.Net.Transports;
using LibplanetConsole.Common;
using Serilog;
using static LibplanetConsole.Seeds.PeerUtility;

namespace LibplanetConsole.Seeds;

internal class Seed(SeedOptions seedOptions)
{
    public static readonly PrivateKey AppProtocolKey = (PrivateKey)GenesisOptions.AppProtocolKey;

    public static readonly AppProtocolVersion AppProtocolVersion
        = AppProtocolVersion.Sign(AppProtocolKey, GenesisOptions.AppProtocolVersion);

    private readonly SeedOptions _seedOptions = seedOptions;
    private readonly ILogger _logger = Log.ForContext<Seed>();

    private ITransport? _transport;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task _task = Task.CompletedTask;
    private Task _refreshTask = Task.CompletedTask;

    public ILogger Logger => _logger;

    public PeerCollection Peers { get; } = new(seedOptions);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_transport is not null)
        {
            throw new InvalidOperationException("Seed node is already running.");
        }

        _cancellationTokenSource
            = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _transport = await CreateTransport(_seedOptions);
        _transport.ProcessMessageHandler.Register(ReceiveMessageAsync);
        _task = _transport.StartAsync(_cancellationTokenSource.Token);
        await _transport.WaitForRunningAsync();
        _refreshTask = RefreshContinuouslyAsync(_cancellationTokenSource.Token);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_transport is null)
        {
            throw new InvalidOperationException("Seed node is not running.");
        }

        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        await _transport.StopAsync(TimeSpan.FromSeconds(0), cancellationToken);
        await _refreshTask;
        await _task;
        _refreshTask = Task.CompletedTask;
        _task = Task.CompletedTask;
        _transport = null;
    }

    private static async Task<NetMQTransport> CreateTransport(SeedOptions seedOptions)
    {
        var privateKey = (PrivateKey)seedOptions.PrivateKey;
        var appProtocolVersion = AppProtocolVersion;
        var appProtocolVersionOptions = new AppProtocolVersionOptions
        {
            AppProtocolVersion = appProtocolVersion,
            TrustedAppProtocolVersionSigners = [],
        };
        var host = seedOptions.EndPoint.Host;
        var port = seedOptions.EndPoint.Port;
        var hostOptions = new HostOptions(host, [], port);
        return await NetMQTransport.Create(privateKey, appProtocolVersionOptions, hostOptions);
    }

    private async Task RefreshContinuouslyAsync(CancellationToken cancellationToken)
    {
        var interval = _seedOptions.RefreshInterval;
        var peers = Peers;
        while (cancellationToken.IsCancellationRequested != true)
        {
            try
            {
                await Task.Delay(interval, cancellationToken);
                await peers.RefreshAsync(cancellationToken);
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

        switch (message.Content)
        {
            case FindNeighborsMsg:
                var alivePeers = peers.Where(item => item.IsAlive)
                                      .Select(item => ToBoundPeer(item.AppPeer))
                                      .ToArray();
                var neighborsMsg = new NeighborsMsg(alivePeers);
                await transport.ReplyMessageAsync(
                    neighborsMsg,
                    messageIdentity,
                    cancellationToken: cancellationToken);
                break;

            default:
                var pongMsg = new PongMsg();
                await transport.ReplyMessageAsync(
                    content: pongMsg,
                    identity: messageIdentity,
                    cancellationToken: cancellationToken);
                break;
        }

        if (message.Remote is BoundPeer boundPeer)
        {
            peers.AddOrUpdate(ToAppPeer(boundPeer), transport);
        }
    }
}
