using System.Net;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Net.Options;
using Libplanet.Net.Transports;
using Libplanet.Net.Consensus;
using Libplanet.Blockchain;
using System.Collections.Immutable;
using Libplanet.Action;
using Libplanet.Types.Tx;
using System.Net.Sockets;
using System.ComponentModel;

namespace OnBoarding.ConsoleHost;

sealed class SwarmHost : IAsyncDisposable
{
    public static readonly PrivateKey AppProtocolKey = PrivateKey.FromString
    (
        "2a15e7deaac09ce631e1faa184efadb175b6b90989cf1faed9dfc321ad1db5ac"
    );

    private readonly PrivateKey _privateKey;
    private readonly string _name;
    private readonly PublicKey[] _validatorKeys;
    private readonly BoundPeer _peer;
    private readonly BoundPeer _consensusPeer;
    private Swarm? _swarm;
    private Task? _startTask;
    private bool _isDisposed;

    public SwarmHost(string name, PrivateKey privateKey, PublicKey[] validatorKeys, int port, int consensusPort)
    {
        _name = name;
        _privateKey = privateKey;
        _validatorKeys = validatorKeys;
        _peer = new BoundPeer(_privateKey.PublicKey, new DnsEndPoint($"{IPAddress.Loopback}", port));
        _consensusPeer = new BoundPeer(_privateKey.PublicKey, new DnsEndPoint($"{IPAddress.Loopback}", consensusPort));
    }

    public string Key => $"{_privateKey.PublicKey}";

    public BoundPeer Peer => _peer;

    public BoundPeer ConsensusPeer => _consensusPeer;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IImmutableSet<BoundPeer> StaticPeers { get; set; } = ImmutableHashSet<BoundPeer>.Empty;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ImmutableList<BoundPeer> ConsensusSeedPeers { get; set; } = ImmutableList<BoundPeer>.Empty;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ImmutableList<BoundPeer> ConsensusPeers { get; set; } = ImmutableList<BoundPeer>.Empty;

    public bool IsRunning => _startTask != null;

    public bool IsDisposed => _isDisposed;

    public Swarm Target => _swarm ?? throw new InvalidOperationException();

    public BlockChain BlockChain => _swarm?.BlockChain ?? throw new InvalidOperationException();

    public override string ToString()
    {
        return $"{_peer.EndPoint}";
    }

    public void StageTransaction(User user, IAction[] actions)
    {
        var blockChain = BlockChain;
        var privateKey = user.PrivateKey;
        var genesisBlock = blockChain.Genesis;
        var nonce = blockChain.GetNextTxNonce(privateKey.ToAddress());
        var values = actions.Select(item => item.PlainValue).ToArray();
        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: genesisBlock.Hash,
            actions: new TxActionList(values)
        );
        blockChain.StageTransaction(transaction);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");
        if (_startTask != null)
            throw new InvalidOperationException("Swarm has been started.");

        var privateKey = _privateKey;
        var validatorKeys = _validatorKeys;
        var name = _name;
        var blockChain = BlockChainUtility.CreateBlockChain(name, validatorKeys);
        var transport = CreateTransport(privateKey, _peer.EndPoint.Port);
        var swarmOptions = new SwarmOptions
        {
            StaticPeers = StaticPeers,
        };
        var consensusTransport = CreateTransport(privateKey, _consensusPeer.EndPoint.Port);
        var consensusReactorOption = new ConsensusReactorOption
        {
            SeedPeers = ConsensusSeedPeers,
            ConsensusPeers = ConsensusPeers,
            ConsensusPort = _consensusPeer.EndPoint.Port,
            ConsensusPrivateKey = privateKey,
            TargetBlockInterval = TimeSpan.FromSeconds(10),
            ContextTimeoutOptions = new(),
        };
        _swarm = new Swarm(blockChain, privateKey, transport, swarmOptions, consensusTransport, consensusReactorOption);
        _startTask = _swarm.StartAsync(cancellationToken: default);
        await Task.CompletedTask;
    }

    private static int[] GetRandomUnusedPorts(int count)
    {
        var ports = new int[count];
        var listeners = new TcpListener[count];
        for (var i = 0; i < count; i++)
        {
            listeners[i] = CreateListener();
            ports[i] = ((IPEndPoint)listeners[i].LocalEndpoint).Port;
        }
        for (var i = 0; i < count; i++)
        {
            listeners[i].Stop();
        }
        return ports;

        static TcpListener CreateListener()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            return listener;
        }
    }
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");
        if (_startTask == null || _swarm == null)
            throw new InvalidOperationException("Swarm has been stopped.");

        await _swarm.StopAsync(cancellationToken: cancellationToken);
        await _startTask;
        _swarm.Dispose();
        _startTask = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");

        if (_swarm != null)
        {
            await _swarm.StopAsync(cancellationToken: default);
            _swarm.Dispose();
        }
        if (_startTask != null)
        {
            await _startTask;
            _startTask = null;
        }

        _isDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? Disposed;

    private static NetMQTransport CreateTransport(PrivateKey privateKey, int port)
    {
        var apv = AppProtocolVersion.Sign(AppProtocolKey, 1);
        var appProtocolVersionOptions = new AppProtocolVersionOptions
        {
            AppProtocolVersion = apv,
        };
        var hostOptions = new HostOptions($"{IPAddress.Loopback}", Array.Empty<IceServer>(), port);
        var task = NetMQTransport.Create(privateKey, appProtocolVersionOptions, hostOptions);
        task.Wait();
        return task.Result;
    }
}
