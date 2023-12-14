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
    private readonly BlockChain _blockChain;
    private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current!;
    private Swarm? _swarm;
    private Task? _startTask;
    private bool _isDisposed;
    private long _blockCount;
    private Task? _pollingTask;
    private CancellationTokenSource? _pollingCancellationTokenSource;

    public SwarmHost(PrivateKey privateKey, BlockChain blockChain, BoundPeer peer, BoundPeer consensusPeer)
    {
        _privateKey = privateKey;
        _blockChain = blockChain;
        _peer = peer;
        _consensusPeer = consensusPeer;
    }

    public BoundPeer Peer => _peer;

    public BoundPeer ConsensusPeer => _consensusPeer;

    public bool IsRunning => _startTask != null;

    public bool IsDisposed => _isDisposed;

    public Swarm Target => _swarm ?? throw new InvalidOperationException();

    public BlockChain BlockChain => _blockChain;

    public override string ToString()
    {
        return $"{_peer.EndPoint}";
    }

    public TxId StageTransaction(User user, IAction[] actions)
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
        return transaction.Id;
    }

    public async Task AddTransactionAsync(User user, IAction[] actions, CancellationToken cancellationToken)
    {
        var blockChain = BlockChain;
        var count = blockChain.Count;
        var id = StageTransaction(user, actions);
        await TaskUtility.WaitIfAsync(() => blockChain.Count <= count, cancellationToken);
        // var block = blockChain.Tip;
        // var execution = blockChain.GetTxExecution(block.Hash, id);
        // if (execution.Fail == true)
        //     throw new InvalidOperationException("Transaction Failed.");
    }

    public async Task StartAsync(BoundPeer seedPeer, BoundPeer consensusSeedPeer, CancellationToken cancellationToken)
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");
        if (_startTask != null)
            throw new InvalidOperationException("Swarm has been started.");

        var privateKey = _privateKey;
        var peer = _peer;
        var consensusPeer = _consensusPeer;
        var blockChain = _blockChain;
        var transport = await CreateTransport(privateKey, peer.EndPoint.Port, cancellationToken);
        var swarmOptions = new SwarmOptions
        {
            StaticPeers = seedPeer == peer ? ImmutableHashSet<BoundPeer>.Empty : ImmutableHashSet.Create(seedPeer),
        };
        var consensusTransport = await CreateTransport(privateKey, consensusPeer.EndPoint.Port, cancellationToken);
        var consensusReactorOption = new ConsensusReactorOption
        {
            SeedPeers = consensusSeedPeer == consensusPeer ? ImmutableList<BoundPeer>.Empty : ImmutableList.Create(consensusSeedPeer),
            ConsensusPort = consensusPeer.EndPoint.Port,
            ConsensusPrivateKey = privateKey,
            TargetBlockInterval = TimeSpan.FromSeconds(10),
            ContextTimeoutOptions = new(),
        };
        _swarm = new Swarm(blockChain, privateKey, transport, swarmOptions, consensusTransport, consensusReactorOption);
        _startTask = _swarm.StartAsync(cancellationToken: default);
        _blockCount = blockChain.Count;
        _pollingCancellationTokenSource = new();
        _pollingTask = Polling(_pollingCancellationTokenSource.Token);
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");
        if (_startTask == null || _swarm == null)
            throw new InvalidOperationException("Swarm has been stopped.");

        _pollingCancellationTokenSource?.Cancel();
        _pollingCancellationTokenSource = null;
        await (_pollingTask ?? Task.CompletedTask);
        await _swarm.StopAsync(cancellationToken: cancellationToken);
        await _startTask;
        _swarm.Dispose();
        _startTask = null;
        _pollingTask = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");

        _pollingCancellationTokenSource?.Cancel();
        _pollingCancellationTokenSource = null;
        await (_pollingTask ?? Task.CompletedTask);
        if (_swarm != null)
        {
            await _swarm.StopAsync(cancellationToken: default);
            _swarm.Dispose();
        }
        await (_startTask ?? Task.CompletedTask);
        _startTask = null;
        _isDisposed = true;
    }

    public event EventHandler? BlockAppended;

    private static async Task<NetMQTransport> CreateTransport(PrivateKey privateKey, int port, CancellationToken cancellationToken)
    {
        var apv = AppProtocolVersion.Sign(AppProtocolKey, 1);
        var appProtocolVersionOptions = new AppProtocolVersionOptions
        {
            AppProtocolVersion = apv,
        };
        var hostOptions = new HostOptions($"{IPAddress.Loopback}", Array.Empty<IceServer>(), port);
        return await NetMQTransport.Create(privateKey, appProtocolVersionOptions, hostOptions);
    }

    private async Task Polling(CancellationToken cancellationToken)
    {
        try
        {
            while (cancellationToken.IsCancellationRequested == false)
            {
                if (_blockCount != _blockChain.Count)
                {
                    _synchronizationContext.Post((s) =>
                    {
                        BlockAppended?.Invoke(this, EventArgs.Empty);
                    }, null);
                    _blockCount = _blockChain.Count;
                }
                await Task.Delay(1, cancellationToken);
            }
        }
        catch (TaskCanceledException)
        {
        }
    }
}
