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

namespace OnBoarding.ConsoleHost;

sealed class SwarmHost : IAsyncDisposable
{
    public static readonly PrivateKey AppProtocolKey = PrivateKey.FromString
    (
        "2a15e7deaac09ce631e1faa184efadb175b6b90989cf1faed9dfc321ad1db5ac"
    );

    private readonly PrivateKey _privateKey;
    private readonly BoundPeer _peer;
    private readonly BoundPeer _consensusPeer;
    private readonly Swarm _swarm;
    private Task? _startTask;
    private bool _isDisposed;

    public SwarmHost(int index, PrivateKey[] validators, BoundPeer[] peers, BoundPeer[] consensusPeers)
    {
        var privateKey = validators[index];
        var peer = peers[index];
        var consensusPeer = consensusPeers[index];
        var validatorKeys = validators.Select(item => item.PublicKey).ToArray();
        var blockChain = BlockChainUtility.CreateBlockChain($"Swarm {index}", validatorKeys);
        var transport = CreateTransport(privateKey, peer.EndPoint.Port);
        var swarmOptions = new SwarmOptions
        {
            StaticPeers = index == 0 ? ImmutableHashSet.Create(peers[1]) : ImmutableHashSet.Create(peers[0])
        };
        var consensusTransport = CreateTransport(privateKey, consensusPeer.EndPoint.Port);
        var consensusReactorOption = new ConsensusReactorOption
        {
            SeedPeers = index == 0 ? ImmutableList.Create(consensusPeers[1]) : ImmutableList.Create(consensusPeers[0]),
            ConsensusPeers = consensusPeers.ToImmutableList(),
            ConsensusPort = consensusPeer.EndPoint.Port,
            ConsensusPrivateKey = privateKey,
            TargetBlockInterval = TimeSpan.FromSeconds(10),
            ContextTimeoutOptions = new(),
        };
        _privateKey = privateKey;
        _peer = peer;
        _consensusPeer = consensusPeer;
        _swarm = new Swarm(blockChain, privateKey, transport, swarmOptions, consensusTransport, consensusReactorOption);
    }

    public string Key => $"{_privateKey.PublicKey}";

    public BoundPeer Peer => _peer;

    public BoundPeer ConsensusPeer => _consensusPeer;

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

        _startTask = _swarm.StartAsync(cancellationToken: default);
        await Task.CompletedTask;
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

        if (_startTask != null)
        {
            await _swarm.StopAsync(cancellationToken: default);
            await _startTask;
            _swarm.Dispose();
            _startTask = null;
        }

        _isDisposed = true;
    }

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
