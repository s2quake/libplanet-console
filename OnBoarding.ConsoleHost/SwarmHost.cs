using System.Net;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Net.Options;
using Libplanet.Net.Transports;
using Libplanet.Net.Consensus;
using Libplanet.Blockchain;
using System.Net.Sockets;
using Libplanet.Types.Blocks;
using System.Collections.Immutable;

namespace OnBoarding.ConsoleHost;

sealed class SwarmHost(PrivateKey privateKey, BlockChain blockChain, BoundPeer[] peers) : IAsyncDisposable
{
    public static readonly PrivateKey GenesisProposer = PrivateKey.FromString
    (
        "2a15e7deaac09ce631e1faa184efadb175b6b90989cf1faed9dfc321ad1db5ac"
    );

    private readonly PrivateKey _privateKey = privateKey;
    private readonly BlockChain _blockChain = blockChain;
    private readonly Swarm _swarm = Create(privateKey, blockChain, peers);
    private Task? _startTask;
    private bool _isDisposed;

    public string Key => $"{_privateKey.PublicKey}";

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

    private static Swarm Create(PrivateKey privateKey, BlockChain blockChain, BoundPeer[] peers)
    {
        var transport = CreateTransport();
        var swarmOptions = new SwarmOptions
        {
            StaticPeers = peers.ToImmutableHashSet(),
        };
        var consensusReactorOption = new ConsensusReactorOption
        {
            SeedPeers = [],
            ConsensusPeers = [],
            ConsensusPort = 0,
            ConsensusPrivateKey = privateKey,
            ConsensusWorkers = 100,
            TargetBlockInterval = TimeSpan.FromSeconds(10),
        };
        return new Swarm(blockChain, privateKey, transport, options: swarmOptions, null, null);
    }

    private static NetMQTransport CreateTransport()
    {
        var port = GetRandomUnusedPort();
        var apv = AppProtocolVersion.Sign(GenesisProposer, 1);
        var appProtocolVersionOptions = new AppProtocolVersionOptions
        {
            AppProtocolVersion = apv,
            // TrustedAppProtocolVersionSigners = publicKeys.ToImmutableHashSet(),
        };
        var hostOptions = new HostOptions($"{IPAddress.Loopback}", Array.Empty<IceServer>(), port);
        var task = NetMQTransport.Create(GenesisProposer, appProtocolVersionOptions, hostOptions);
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

    // internal async Task TestAsync(SwarmHostCollection swarmHosts)
    // {
    //     var peers = swarmHosts.Where(item => item != this).Select(item => item._swarm.AsPeer).ToArray();
    //     foreach (var item in peers)
    //     {
    //         await _swarm.AddPeersAsync([item], TimeSpan.FromSeconds(10));
    //     }
    // }

    internal void Broadcast(Block block)
    {
        _swarm.BroadcastBlock(block);
    }
}
