using System.ComponentModel;
using System.Net;
using System.Text.Json.Serialization;
using Dasync.Collections;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Net.Messages;
using Libplanet.Net.Options;
using Libplanet.Net.Transports;
using LibplanetConsole.Common;
using LibplanetConsole.Seed.Converters;
using Serilog;

namespace LibplanetConsole.Seed;

public sealed class SeedNode(SeedOptions seedOptions)
{
    static SeedNode()
    {
        TypeDescriptor.AddAttributes(
            typeof(BoundPeer), new JsonConverterAttribute(typeof(BoundPeerJsonConverter)));
    }

    public static readonly PrivateKey AppProtocolKey
        = new("2a15e7deaac09ce631e1faa184efadb175b6b90989cf1faed9dfc321ad1db5ac");

    public static readonly AppProtocolVersion AppProtocolVersion
        = AppProtocolVersion.Sign((PrivateKey)AppProtocolKey, 1);

    private readonly ILogger _logger = Log.ForContext<SeedNode>();

    private ITransport? _transport;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task _task = Task.CompletedTask;
    private Task _refreshTask = Task.CompletedTask;

    public ILogger Logger => _logger;

    public bool IsRunning { get; private set; }

    public PeerCollection Peers { get; } = new(seedOptions);

    public BoundPeer BoundPeer => new(
        seedOptions.PrivateKey.PublicKey, (DnsEndPoint)seedOptions.EndPoint);

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
        _refreshTask = RefreshContinuouslyAsync(_cancellationTokenSource.Token);
        IsRunning = true;
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
        var interval = seedOptions.RefreshInterval;
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
                                      .Select(item => item.BoundPeer)
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
            peers.AddOrUpdate(boundPeer, transport);
        }
    }
}
