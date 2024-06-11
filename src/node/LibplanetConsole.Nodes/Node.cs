using System.Collections.Concurrent;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Renderers;
using Libplanet.Common;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Net.Consensus;
using Libplanet.Net.Options;
using Libplanet.Net.Transports;
using Libplanet.Types.Blocks;
using Libplanet.Types.Tx;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Nodes.Serializations;
using LibplanetConsole.Seeds;
using Serilog;

namespace LibplanetConsole.Nodes;

internal sealed class Node : IActionRenderer, INode, IApplicationService
{
    private readonly SecureString _privateKey;
    private readonly string _storePath;
    private readonly SynchronizationContext _synchronizationContext
        = SynchronizationContext.Current!;

    private readonly PrivateKey _seedNodePrivateKey = new();
    private readonly ConcurrentDictionary<TxId, ManualResetEvent> _eventByTxId = [];
    private readonly ConcurrentDictionary<IValue, Exception> _exceptionByAction = [];
    private readonly EndPoint? _seedEndPoint;
    private readonly ManualResetEvent _initializedResetEvent = new(false);
    private readonly ILogger _logger;

    private DnsEndPoint? _blocksyncEndPoint;
    private DnsEndPoint? _consensusEndPoint;
    private Swarm? _swarm;
    private Task _startTask = Task.CompletedTask;
    private bool _isDisposed;
    private SeedNode? _blocksyncSeedNode;
    private SeedNode? _consensusSeedNode;
    private NodeOptions _nodeOptions;

    public Node(ApplicationOptions options, ILogger logger)
    {
        _seedEndPoint = options.NodeEndPoint;
        _privateKey = PrivateKeyUtility.ToSecureString(options.PrivateKey);
        _storePath = options.StorePath;
        PublicKey = options.PrivateKey.PublicKey;
        _logger = logger;
        _nodeOptions = new NodeOptions
        {
            GenesisOptions = new GenesisOptions
            {
                GenesisKey = new(),
                GenesisValidators = options.GenesisValidators,
                Timestamp = DateTimeOffset.UtcNow,
            },
        };
        UpdateNodeInfo();
        _logger.Debug("Node is created: {Address}", Address);
    }

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public DnsEndPoint SwarmEndPoint
    {
        get => _blocksyncEndPoint ?? throw new InvalidOperationException();
        set => _blocksyncEndPoint = value;
    }

    public DnsEndPoint ConsensusEndPoint
    {
        get => _consensusEndPoint ?? throw new InvalidOperationException();
        set => _consensusEndPoint = value;
    }

    public string StorePath => _storePath;

    public bool IsRunning { get; private set; }

    public bool IsDisposed => _isDisposed;

    public PublicKey PublicKey { get; }

    public Address Address => PublicKey.Address;

    public BlockChain BlockChain => _swarm?.BlockChain ?? throw new InvalidOperationException();

    public NodeInfo Info { get; private set; } = NodeInfo.Empty;

    public BoundPeer[] Peers
    {
        get
        {
            if (_swarm is not null)
            {
                return [.. _swarm.Peers];
            }

            throw new InvalidOperationException();
        }
    }

    public BoundPeer BlocksyncSeedPeer
        => _blocksyncSeedNode?.BoundPeer ?? NodeOptions.BlocksyncSeedPeer ??
            throw new InvalidOperationException();

    public BoundPeer ConsensusSeedPeer
        => _consensusSeedNode?.BoundPeer ?? NodeOptions.ConsensusSeedPeer ??
            throw new InvalidOperationException();

    public NodeOptions NodeOptions => _nodeOptions;

    public override string ToString() => AddressUtility.ToString(Address);

    public bool Verify(object obj, byte[] signature)
        => PublicKeyUtility.Verify(PublicKey, obj, signature);

    public Task<TxId> AddTransactionAsync(IAction[] actions, CancellationToken cancellationToken)
        => AddTransactionAsync(
            PrivateKeyUtility.FromSecureString(_privateKey), actions, cancellationToken);

    public async Task<TxId> AddTransactionAsync(
        PrivateKey privateKey, IAction[] actions, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

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
        await AddTransactionAsync(transaction, cancellationToken);
        return transaction.Id;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning == true, "Node is already running.");
        if (_initializedResetEvent.WaitOne(10000) != true)
        {
            throw new InvalidOperationException("NodeOptions is not initialized.");
        }

        var privateKey = PrivateKeyUtility.FromSecureString(_privateKey);
        var nodeOptions = NodeOptions;
        var storePath = _storePath;
        var blocksyncEndPoint = _blocksyncEndPoint ?? DnsEndPointUtility.Next();
        var consensusEndPoint = _consensusEndPoint ?? DnsEndPointUtility.Next();
        var blocksyncSeedPeer = nodeOptions.BlocksyncSeedPeer ??
            new BoundPeer(_seedNodePrivateKey.PublicKey, DnsEndPointUtility.Next());
        var consensusSeedPeer = nodeOptions.ConsensusSeedPeer ??
            new BoundPeer(_seedNodePrivateKey.PublicKey, DnsEndPointUtility.Next());
        var swarmTransport
            = await CreateTransport(privateKey, blocksyncEndPoint, cancellationToken);
        var swarmOptions = new SwarmOptions
        {
            StaticPeers = [blocksyncSeedPeer],
            BootstrapOptions = new()
            {
                SeedPeers = [blocksyncSeedPeer],
            },
        };
        var consensusTransport = await CreateTransport(
            privateKey: privateKey,
            endPoint: consensusEndPoint,
            cancellationToken: cancellationToken);
        var consensusReactorOption = new ConsensusReactorOption
        {
            SeedPeers = [consensusSeedPeer],
            ConsensusPort = consensusEndPoint.Port,
            ConsensusPrivateKey = privateKey,
            TargetBlockInterval = TimeSpan.FromSeconds(2),
            ContextTimeoutOptions = new(),
        };
        var blockChain = BlockChainUtility.CreateBlockChain(
            genesisOptions: nodeOptions.GenesisOptions,
            storePath: storePath,
            renderer: this);

        if (nodeOptions.BlocksyncSeedPeer is null)
        {
            _blocksyncSeedNode = new SeedNode(new()
            {
                PrivateKey = _seedNodePrivateKey,
                EndPoint = blocksyncSeedPeer.EndPoint,
                AppProtocolVersion = BlockChainUtility.AppProtocolVersion,
            });
            await _blocksyncSeedNode.StartAsync(cancellationToken);
            _logger.Debug("Node.BlocksyncSeed is started: {Address}", Address);
        }

        if (nodeOptions.ConsensusSeedPeer is null)
        {
            _consensusSeedNode = new SeedNode(new()
            {
                PrivateKey = _seedNodePrivateKey,
                EndPoint = consensusSeedPeer.EndPoint,
                AppProtocolVersion = BlockChainUtility.AppProtocolVersion,
            });
            await _consensusSeedNode.StartAsync(cancellationToken);
            _logger.Debug("Node.ConsensusSeed is started: {Address}", Address);
        }

        _blocksyncEndPoint = blocksyncEndPoint;
        _consensusEndPoint = consensusEndPoint;
        _swarm = new Swarm(
            blockChain: blockChain,
            privateKey: privateKey,
            transport: swarmTransport,
            options: swarmOptions,
            consensusTransport: consensusTransport,
            consensusOption: consensusReactorOption);
        _startTask = _swarm.StartAsync(cancellationToken: default);
        _logger.Debug("Node.Swarm is starting: {Address}", Address);
        await _swarm.BootstrapAsync(cancellationToken: default);
        _logger.Debug("Node.Swarm is bootstrapped: {Address}", Address);
        IsRunning = true;
        UpdateNodeInfo();
        _logger.Debug("Node is started: {Address}", Address);
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        if (_consensusSeedNode is not null)
        {
            await _consensusSeedNode.StopAsync(cancellationToken: default);
            _consensusSeedNode = null;
            _logger.Debug("Node.ConsensusSeed is stopped: {Address}", Address);
        }

        if (_blocksyncSeedNode is not null)
        {
            await _blocksyncSeedNode!.StopAsync(cancellationToken: default);
            _blocksyncSeedNode = null;
            _logger.Debug("Node.BlocksyncSeed is stopped: {Address}", Address);
        }

        if (_swarm is not null)
        {
            await _swarm.StopAsync(cancellationToken: cancellationToken);
            await _startTask;
            _logger.Debug("Node.Swarm is stopping: {Address}", Address);
            _swarm.Dispose();
            _logger.Debug("Node.Swarm is stopped: {Address}", Address);
        }

        _blocksyncEndPoint = null;
        _consensusEndPoint = null;
        _swarm = null;
        _startTask = Task.CompletedTask;
        IsRunning = false;
        UpdateNodeInfo();
        _logger.Debug("Node is stopped: {Address}", Address);
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async Task AddTransactionAsync(
        Transaction transaction, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        _logger.Debug("Node adds a transaction: {TxId}", transaction.Id);
        var blockChain = BlockChain;
        var height = blockChain.Tip.Index + 1;
        var manualResetEvent = _eventByTxId.GetOrAdd(transaction.Id, _ =>
        {
            return new ManualResetEvent(initialState: false);
        });
        blockChain.StageTransaction(transaction);
        await Task.Run(manualResetEvent.WaitOne, cancellationToken);

        _eventByTxId.TryRemove(transaction.Id, out _);

        var sb = new StringBuilder();
        foreach (var item in transaction.Actions)
        {
            if (_exceptionByAction.TryRemove(item, out var exception) == true &&
                exception is UnexpectedlyTerminatedActionException)
            {
                sb.AppendLine($"{exception.InnerException}");
            }
        }

        if (sb.Length > 0)
        {
            throw new InvalidOperationException(sb.ToString());
        }

        _logger.Debug("Node added a transaction: {TxId}", transaction.Id);
    }

    public long GetNextNonce(Address address)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        var blockChain = BlockChain;
        var nonce = blockChain.GetNextTxNonce(address);
        return nonce;
    }

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        DnsEndPointUtility.Release(ref _blocksyncEndPoint);
        DnsEndPointUtility.Release(ref _consensusEndPoint);

        if (_swarm is not null)
        {
            await _swarm.StopAsync(cancellationToken: default);
            _swarm.Dispose();
        }

        if (_consensusSeedNode is not null)
        {
            await _consensusSeedNode.StopAsync(cancellationToken: default);
            _consensusSeedNode = null;
        }

        if (_blocksyncSeedNode is not null)
        {
            await _blocksyncSeedNode.StopAsync(cancellationToken: default);
            _blocksyncSeedNode = null;
        }

        await (_startTask ?? Task.CompletedTask);
        _startTask = Task.CompletedTask;
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    async Task IApplicationService.InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var nodeOptions = _seedEndPoint != null
            ? await NodeOptions.CreateAsync(_seedEndPoint, cancellationToken)
            : NodeOptions;
        _nodeOptions = nodeOptions;
        _initializedResetEvent.Set();
    }

    void IRenderer.RenderBlock(Block oldTip, Block newTip)
    {
        _synchronizationContext.Post(Action, state: null);

        void Action(object? state)
        {
            var height = newTip.Index;
            foreach (var transaction in newTip.Transactions)
            {
                if (_eventByTxId.TryGetValue(transaction.Id, out var manualResetEvent) == true)
                {
                    manualResetEvent.Set();
                }
            }

            var blockChain = _swarm!.BlockChain;
            var blockInfo = new BlockInfo(blockChain.Tip);
            UpdateNodeInfo();
            BlockAppended?.Invoke(this, new(blockInfo));
        }
    }

    void IActionRenderer.RenderAction(
        IValue action, ICommittedActionContext context, HashDigest<SHA256> nextState)
    {
    }

    void IActionRenderer.RenderActionError(
        IValue action, ICommittedActionContext context, Exception exception)
    {
        _exceptionByAction.AddOrUpdate(action, exception, (_, _) => exception);
    }

    void IActionRenderer.RenderBlockEnd(Block oldTip, Block newTip)
    {
    }

    private static async Task<NetMQTransport> CreateTransport(
        PrivateKey privateKey, DnsEndPoint endPoint, CancellationToken cancellationToken)
    {
        var appProtocolVersionOptions = new AppProtocolVersionOptions
        {
            AppProtocolVersion = BlockChainUtility.AppProtocolVersion,
        };
        var hostOptions = new HostOptions(endPoint.Host, [], endPoint.Port);
        return await NetMQTransport.Create(privateKey, appProtocolVersionOptions, hostOptions);
    }

    private void UpdateNodeInfo()
    {
        var nodeInfo = new NodeInfo
        {
            ProcessId = Environment.ProcessId,
            Address = Address,
            AppProtocolVersion = $"{BlockChainUtility.AppProtocolVersion}",
        };

        if (IsRunning == true)
        {
            nodeInfo = nodeInfo with
            {
                SwarmEndPoint = DnsEndPointUtility.ToString(SwarmEndPoint),
                ConsensusEndPoint = DnsEndPointUtility.ToString(ConsensusEndPoint),
                GenesisHash = BlockChain.Genesis.Hash,
                TipHash = BlockChain.Tip.Hash,
                IsRunning = IsRunning,
                Peers = [.. Peers.Select(peer => new BoundPeerInfo(peer))],
            };
        }

        Info = nodeInfo;
    }
}
