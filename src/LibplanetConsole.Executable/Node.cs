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
using Libplanet.Blockchain.Renderers;
using Libplanet.Types.Blocks;
using Bencodex.Types;
using Libplanet.Common;
using System.Security.Cryptography;
using LibplanetConsole.Executable.Exceptions;
using System.Collections.Concurrent;
using System.Text;

namespace LibplanetConsole.Executable;

sealed class Node : IAsyncDisposable, IActionRenderer
{
    public static readonly PrivateKey AppProtocolKey = PrivateKey.FromString
    (
        "2a15e7deaac09ce631e1faa184efadb175b6b90989cf1faed9dfc321ad1db5ac"
    );

    private readonly string _name;
    private readonly PrivateKey _privateKey;
    private readonly BoundPeer _peer;
    private readonly BoundPeer _consensusPeer;
    private readonly BlockChain _blockChain;
    private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current!;
    private readonly ConcurrentDictionary<long, ManualResetEvent> _eventByHeight = [];
    private readonly ConcurrentDictionary<IValue, Exception> _exceptionByAction = [];
    private Swarm? _swarm;
    private Task? _startTask;
    private bool _isDisposed;

    public Node(string name, PrivateKey privateKey, PublicKey[] validatorKeys, string storePath)
    {
        _name = name;
        _privateKey = privateKey;
        _peer = new BoundPeer(privateKey.PublicKey, new DnsEndPoint($"{IPAddress.Loopback}", PortUtility.GetPort()));
        _consensusPeer = new BoundPeer(privateKey.PublicKey, new DnsEndPoint($"{IPAddress.Loopback}", PortUtility.GetPort()));
        _blockChain = BlockChainUtility.CreateBlockChain(validatorKeys, storePath, this);
    }

    public BoundPeer Peer => _peer;

    public BoundPeer ConsensusPeer => _consensusPeer;

    public bool IsRunning => _startTask != null;

    public bool IsDisposed => _isDisposed;

    public Address Address => _privateKey.Address;

    public PublicKey PublicKey => _privateKey.PublicKey;

    public Swarm Target => _swarm ?? throw new InvalidOperationException("Swarm has been stopped.");

    public BlockChain BlockChain => _blockChain;

    public string Name => _name;

    public string Identifier { get; internal set; } = string.Empty;

    public override string ToString()
    {
        return $"{_peer.EndPoint}";
    }

    public Task<Block> AddTransactionAsync(IAction[] actions, CancellationToken cancellationToken)
        => AddTransactionAsync(_privateKey, actions, cancellationToken);

    public Task<Block> AddTransactionAsync(Client client, IAction[] actions, CancellationToken cancellationToken)
        => AddTransactionAsync(client.PrivateKey, actions, cancellationToken);

    public async Task<Block> AddTransactionAsync(PrivateKey privateKey, IAction[] actions, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(_startTask == null || _swarm == null, "Swarm has been stopped.");

        var blockChain = BlockChain;
        var genesisBlock = blockChain.Genesis;
        var nonce = blockChain.GetNextTxNonce(privateKey.Address);
        var values = actions.Select(item => item.PlainValue).ToArray();
        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: genesisBlock.Hash,
            actions: new TxActionList(values)
        );
        var height = blockChain.Tip.Index + 1;
        var manualResetEvent = _eventByHeight.GetOrAdd(height, (_) => new ManualResetEvent(initialState: false));
        blockChain.StageTransaction(transaction);
        await Task.Run(manualResetEvent.WaitOne, cancellationToken);

        var sb = new StringBuilder();
        foreach (var item in values)
        {
            if (_exceptionByAction.TryRemove(item, out var exception) == true && exception is UnexpectedlyTerminatedActionException)
            {
                sb.AppendLine($"{exception.InnerException}");
            }
        }
        if (sb.Length > 0)
        {
            throw new InvalidOperationException(sb.ToString());
        }

        return blockChain[height];
    }

    public Task StartAsync(CancellationToken cancellationToken) => StartAsync(_peer, _consensusPeer, cancellationToken);

    public async Task StartAsync(BoundPeer seedPeer, BoundPeer consensusSeedPeer, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(_startTask != null, "Swarm has been started.");

        var privateKey = _privateKey;
        var peer = _peer;
        var consensusPeer = _consensusPeer;
        var blockChain = _blockChain;
        var transport = await CreateTransport(privateKey, peer.EndPoint.Port, cancellationToken);
        var swarmOptions = new SwarmOptions
        {
            StaticPeers = seedPeer == peer ? [] : ImmutableHashSet.Create(seedPeer),
        };
        var consensusTransport = await CreateTransport(privateKey, consensusPeer.EndPoint.Port, cancellationToken);
        var consensusReactorOption = new ConsensusReactorOption
        {
            SeedPeers = consensusSeedPeer == consensusPeer ? [] : [consensusSeedPeer],
            ConsensusPort = consensusPeer.EndPoint.Port,
            ConsensusPrivateKey = privateKey,
            TargetBlockInterval = TimeSpan.FromSeconds(2),
            ContextTimeoutOptions = new(),
        };
        _swarm = new Swarm(blockChain, privateKey, transport, swarmOptions, consensusTransport, consensusReactorOption);
        _startTask = _swarm.StartAsync(cancellationToken: default);
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(_startTask == null || _swarm == null, "Swarm has been stopped.");

        await _swarm!.StopAsync(cancellationToken: cancellationToken);
        await _startTask!;
        _swarm.Dispose();
        _startTask = null;
    }

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        if (_swarm != null)
        {
            await _swarm.StopAsync(cancellationToken: default);
            _swarm.Dispose();
        }
        await (_startTask ?? Task.CompletedTask);
        _startTask = null;
        _isDisposed = true;
        PortUtility.ReleasePort(_peer.EndPoint.Port);
        PortUtility.ReleasePort(_consensusPeer.EndPoint.Port);
    }

    public event EventHandler? BlockAppended;

    private static async Task<NetMQTransport> CreateTransport(PrivateKey privateKey, int port, CancellationToken cancellationToken)
    {
        var apv = AppProtocolVersion.Sign(AppProtocolKey, 1);
        var appProtocolVersionOptions = new AppProtocolVersionOptions
        {
            AppProtocolVersion = apv,
        };
        var hostOptions = new HostOptions($"{IPAddress.Loopback}", [], port);
        return await NetMQTransport.Create(privateKey, appProtocolVersionOptions, hostOptions);
    }

    #region IRenderer

    void IRenderer.RenderBlock(Block oldTip, Block newTip)
    {
        var height = newTip.Index;
        if (_eventByHeight.TryGetValue(height, out var manualResetEvent) == true)
        {
            manualResetEvent.Set();
        }
        _synchronizationContext.Post((s) =>
        {
            BlockAppended?.Invoke(this, EventArgs.Empty);
        }, null);
    }

    void IActionRenderer.RenderAction(IValue action, ICommittedActionContext context, HashDigest<SHA256> nextState)
    {

    }

    void IActionRenderer.RenderActionError(IValue action, ICommittedActionContext context, Exception exception)
    {
        _exceptionByAction.AddOrUpdate(action, exception, (_, _) => exception);
    }

    void IActionRenderer.RenderBlockEnd(Block oldTip, Block newTip)
    {
    }

    #endregion
}
