using System.Collections.Concurrent;
using System.Security;
using System.Security.Cryptography;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.Loader;
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
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Seeds;
using Serilog;
using static LibplanetConsole.Nodes.PeerUtility;

namespace LibplanetConsole.Nodes;

internal sealed partial class Node : IActionRenderer, INode, IApplicationService
{
    private readonly SecureString _privateKey;
    private readonly string _storePath;
    private readonly SynchronizationContext _synchronizationContext
        = SynchronizationContext.Current!;

    private readonly IServiceProvider _serviceProvider;
    private readonly AppPrivateKey _seedNodePrivateKey = new();
    private readonly ConcurrentDictionary<TxId, ManualResetEvent> _eventByTxId = [];
    private readonly ConcurrentDictionary<IValue, Exception> _exceptionByAction = [];
    private readonly AppEndPoint? _seedEndPoint;
    private readonly ManualResetEvent _initializedResetEvent = new(false);
    private readonly ILogger _logger;

    private AppEndPoint? _blocksyncEndPoint;
    private AppEndPoint? _consensusEndPoint;
    private Swarm? _swarm;
    private Task _startTask = Task.CompletedTask;
    private bool _isDisposed;
    private SeedNode? _blocksyncSeedNode;
    private SeedNode? _consensusSeedNode;
    private NodeOptions _nodeOptions;

    public Node(IServiceProvider serviceProvider, ApplicationOptions options, ILogger logger)
    {
        _serviceProvider = serviceProvider;
        _seedEndPoint = options.NodeEndPoint;
        _privateKey = options.PrivateKey.ToSecureString();
        _storePath = options.StorePath;
        PublicKey = options.PrivateKey.PublicKey;
        _logger = logger;
        _nodeOptions = new NodeOptions
        {
            GenesisOptions = new GenesisOptions
            {
                GenesisKey = (AppPrivateKey)BlockChainUtility.AppProtocolKey,
                GenesisValidators = options.Validators,
                Timestamp = DateTimeOffset.UtcNow,
            },
        };
        UpdateNodeInfo();
        _logger.Debug("Node is created: {Address}", Address);
    }

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public AppEndPoint SwarmEndPoint
    {
        get => _blocksyncEndPoint ?? throw new InvalidOperationException();
        set => _blocksyncEndPoint = value;
    }

    public AppEndPoint ConsensusEndPoint
    {
        get => _consensusEndPoint ?? throw new InvalidOperationException();
        set => _consensusEndPoint = value;
    }

    public string StorePath => _storePath;

    public bool IsRunning { get; private set; }

    public bool IsDisposed => _isDisposed;

    public AppPublicKey PublicKey { get; }

    public AppAddress Address => PublicKey.Address;

    public BlockChain BlockChain => _swarm?.BlockChain ?? throw new InvalidOperationException();

    public NodeInfo Info { get; private set; } = NodeInfo.Empty;

    public Swarm Swarm => _swarm ?? throw new InvalidOperationException();

    public AppPeer[] Peers
    {
        get
        {
            if (_swarm is not null)
            {
                return [.. _swarm.Peers.Select(item => ToAppPeer(item))];
            }

            throw new InvalidOperationException();
        }
    }

    public AppPeer BlocksyncSeedPeer
        => _blocksyncSeedNode?.BoundPeer ?? NodeOptions.BlocksyncSeedPeer ??
            throw new InvalidOperationException();

    public AppPeer ConsensusSeedPeer
        => _consensusSeedNode?.BoundPeer ?? NodeOptions.ConsensusSeedPeer ??
            throw new InvalidOperationException();

    public NodeOptions NodeOptions => _nodeOptions;

    public override string ToString() => $"{Address:S}";

    public object? GetService(Type serviceType)
    {
        if (serviceType == typeof(Swarm))
        {
            return _swarm;
        }

        if (serviceType == typeof(BlockChain))
        {
            return BlockChain;
        }

        return _serviceProvider.GetService(serviceType);
    }

    public bool Verify(object obj, byte[] signature) => PublicKey.Verify(obj, signature);

    public byte[] Sign(object obj) => AppPrivateKey.FromSecureString(_privateKey).Sign(obj);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning == true, "Node is already running.");
        if (_initializedResetEvent.WaitOne(10000) != true)
        {
            throw new InvalidOperationException("NodeOptions is not initialized.");
        }

        var privateKey = (PrivateKey)AppPrivateKey.FromSecureString(_privateKey);
        var nodeOptions = NodeOptions;
        var storePath = _storePath;
        var blocksyncEndPoint = _blocksyncEndPoint ?? AppEndPoint.Next();
        var consensusEndPoint = _consensusEndPoint ?? AppEndPoint.Next();
        var blocksyncSeedPeer = nodeOptions.BlocksyncSeedPeer ??
            new AppPeer(_seedNodePrivateKey.PublicKey, AppEndPoint.Next());
        var consensusSeedPeer = nodeOptions.ConsensusSeedPeer ??
            new AppPeer(_seedNodePrivateKey.PublicKey, AppEndPoint.Next());
        var swarmTransport
            = await CreateTransport(privateKey, blocksyncEndPoint);
        var swarmOptions = new SwarmOptions
        {
            StaticPeers = [ToBoundPeer(blocksyncSeedPeer)],
            BootstrapOptions = new()
            {
                SeedPeers = [ToBoundPeer(blocksyncSeedPeer)],
            },
        };
        var consensusTransport = await CreateTransport(
            privateKey: privateKey,
            endPoint: consensusEndPoint);
        var consensusReactorOption = new ConsensusReactorOption
        {
            SeedPeers = [ToBoundPeer(consensusSeedPeer)],
            ConsensusPort = consensusEndPoint.Port,
            ConsensusPrivateKey = privateKey,
            TargetBlockInterval = TimeSpan.FromSeconds(2),
            ContextTimeoutOptions = new(),
        };
        var actionLoaders = CollectActionLoaders(_serviceProvider);
        var blockChain = BlockChainUtility.CreateBlockChain(
            genesisOptions: nodeOptions.GenesisOptions,
            storePath: storePath,
            renderer: this,
            actionLoaders: actionLoaders);

        if (nodeOptions.BlocksyncSeedPeer is null)
        {
            _blocksyncSeedNode = new SeedNode(new()
            {
                PrivateKey = _seedNodePrivateKey,
                EndPoint = blocksyncSeedPeer.EndPoint,
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

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        _blocksyncEndPoint = null;
        _consensusEndPoint = null;

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
        var nodeOptions = _seedEndPoint is not null
            ? await NodeOptions.CreateAsync(_seedEndPoint, cancellationToken)
            : NodeOptions;
        _nodeOptions = nodeOptions;
        _initializedResetEvent.Set();
    }

    void IRenderer.RenderBlock(Block oldTip, Block newTip)
    {
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
        _synchronizationContext.Post(Action, state: null);

        void Action(object? state)
        {
            foreach (var transaction in newTip.Transactions)
            {
                if (_eventByTxId.TryGetValue(transaction.Id, out var manualResetEvent) == true)
                {
                    manualResetEvent.Set();
                }
            }

            var blockChain = _swarm!.BlockChain;
            var blockInfo = new BlockInfo(blockChain, blockChain.Tip);
            UpdateNodeInfo();
            BlockAppended?.Invoke(this, new(blockInfo));
        }
    }

    private static IActionLoader[] CollectActionLoaders(IServiceProvider serviceProvider)
    {
        var actionLoaderProviders
            = serviceProvider.GetService<IEnumerable<IActionLoaderProvider>>();
        var actionLoaderList
            = actionLoaderProviders.Select(item => item.GetActionLoader()).ToList();
        actionLoaderList.Add(new AssemblyActionLoader(typeof(AssemblyActionLoader).Assembly));
        return [.. actionLoaderList];
    }

    private static async Task<NetMQTransport> CreateTransport(
        PrivateKey privateKey, AppEndPoint endPoint)
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
                SwarmEndPoint = AppEndPoint.ToString(SwarmEndPoint),
                ConsensusEndPoint = AppEndPoint.ToString(ConsensusEndPoint),
                GenesisHash = (AppHash)BlockChain.Genesis.Hash,
                TipHash = (AppHash)BlockChain.Tip.Hash,
                IsRunning = IsRunning,
                Peers = [.. Peers],
            };
        }

        Info = nodeInfo;
    }
}
