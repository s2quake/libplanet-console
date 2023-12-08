using System.Net;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Net.Options;
using Libplanet.Net.Transports;
using Libplanet.Net.Consensus;
using Libplanet.Blockchain;
using System.Net.Sockets;
using System.Collections.Immutable;

namespace OnBoarding.ConsoleHost;

sealed class SwarmHost(User user, PublicKey[] validatorKeys, BoundPeer[] peers, BoundPeer[] consensusPeers) : IAsyncDisposable
{
    public static readonly PrivateKey AppProtocolKey = PrivateKey.FromString
    (
        "2a15e7deaac09ce631e1faa184efadb175b6b90989cf1faed9dfc321ad1db5ac"
    );

    private readonly User _user = user;
    private readonly Swarm _swarm = Create(user, validatorKeys, peers, consensusPeers);
    private Task? _startTask;
    private bool _isDisposed;

    public string Key => $"{_user.PublicKey}";

    public bool IsRunning => _startTask != null;

    public bool IsDisposed => _isDisposed;

    public Swarm Target => _swarm;

    public BlockChain BlockChain => _swarm.BlockChain;

    public override string ToString()
    {
        return $"{_swarm.EndPoint.Host}:{_swarm.EndPoint.Port}";
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(condition: _isDisposed, this);
        if (_startTask != null)
            throw new InvalidOperationException("Swarm has been started.");

        _startTask = _swarm.StartAsync(cancellationToken: default);
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(condition: _isDisposed, this);
        if (_startTask == null)
            throw new InvalidOperationException("Swarm has been stopped.");

        await _swarm.StopAsync(cancellationToken: cancellationToken);
        await _startTask;
        _swarm.Dispose();
        _startTask = null;
    }

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedException.ThrowIf(condition: _isDisposed, this);

        if (_startTask != null)
        {
            await _swarm.StopAsync(cancellationToken: default);
            await _startTask;
            _swarm.Dispose();
            _startTask = null;
        }

        _isDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? Disposed;

    private static Swarm Create(User user, PublicKey[] validatorKeys, BoundPeer[] peers, BoundPeer[] consensusPeers)
    {
        var staticPeers = peers.Where(item => item != user.Peer).ToArray();
        var blockChain = BlockChainUtils.CreateBlockChain(user.Name, validatorKeys);
        var privateKey = user.PrivateKey;
        var transport = CreateTransport(privateKey, user.Peer.EndPoint.Port);
        var bootstrapOptions = new BootstrapOptions
        {
            SeedPeers = [.. staticPeers],
        };
        var swarmOptions = new SwarmOptions
        {
            StaticPeers = staticPeers.ToImmutableHashSet(),
            BootstrapOptions = bootstrapOptions,
        };
        var consensusTransport = CreateTransport(privateKey, user.ConsensusPeer.EndPoint.Port);
        var consensusReactorOption = new ConsensusReactorOption
        {
            SeedPeers = [.. staticPeers],
            ConsensusPeers = [.. consensusPeers],
            ConsensusPort = user.ConsensusPeer.EndPoint.Port,
            ConsensusPrivateKey = privateKey,
            ConsensusWorkers = 100,
            TargetBlockInterval = TimeSpan.FromSeconds(10),
            ContextTimeoutOptions = new(),
        };
        return new Swarm(blockChain, privateKey, transport, swarmOptions, consensusTransport, consensusReactorOption);
    }

    private static NetMQTransport CreateTransport(PrivateKey privateKey, int port)
    {
        var apv = AppProtocolVersion.Sign(AppProtocolKey, 1);
        var appProtocolVersionOptions = new AppProtocolVersionOptions
        {
            AppProtocolVersion = apv,
            // TrustedAppProtocolVersionSigners = publicKeys.ToImmutableHashSet(),
        };
        var hostOptions = new HostOptions($"{IPAddress.Loopback}", Array.Empty<IceServer>(), port);
        var task = NetMQTransport.Create(privateKey, appProtocolVersionOptions, hostOptions);
        task.Wait();
        return task.Result;
    }

    private static int GetRandomUnusedPort()
    {
        var listener = CreateListener();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;

        static TcpListener CreateListener()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            return listener;
        }
    }
}
