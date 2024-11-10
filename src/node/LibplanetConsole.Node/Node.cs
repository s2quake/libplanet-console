using System.Collections.Concurrent;
using System.Security.Cryptography;
using Grpc.Net.Client;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Renderers;
using Libplanet.Net;
using Libplanet.Net.Consensus;
using Libplanet.Net.Options;
using Libplanet.Net.Transports;
using Libplanet.Store;
using Libplanet.Store.Trie;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Seed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LibplanetConsole.Node;

internal sealed partial class Node : IActionRenderer, INode, IAsyncDisposable
{
    private readonly PrivateKey _privateKey;
    private readonly string _storePath;
    private readonly SynchronizationContext _synchronizationContext
        = SynchronizationContext.Current!;

    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<TxId, ManualResetEvent> _eventByTxId = [];
    private readonly ConcurrentDictionary<IValue, Exception> _exceptionByAction = [];
    private readonly ILogger _logger;
    private readonly Block _genesisBlock;
    private readonly AppProtocolVersion _appProtocolVersion;
    private readonly IActionProvider _actionProvider;
    private readonly int _blocksyncPort;
    private readonly int _consensusPort;

    private EndPoint? _seedEndPoint;
    private Swarm? _swarm;
    private IKeyValueStore? _keyValueStore;
    private IStore? _store;
    private IStateStore? _stateStore;
    private Task _startTask = Task.CompletedTask;
    private INodeContent[]? _contents;
    private bool _isDisposed;

    public Node(
        IServiceProvider serviceProvider,
        IApplicationOptions options,
        IOptions<NodeOptions> nodeOptions)
    {
        _serviceProvider = serviceProvider;
        _seedEndPoint = options.SeedEndPoint;
        _privateKey = options.PrivateKey;
        _logger = serviceProvider.GetLogger<Node>();
        _logger.LogDebug("Node is creating...: {Address}", options.PrivateKey.Address);
        _storePath = options.StorePath;
        PublicKey = options.PrivateKey.PublicKey;
        _actionProvider = ModuleLoader.LoadActionLoader(
            options.ActionProviderModulePath, options.ActionProviderType);
        _genesisBlock = options.GenesisBlock;
        _appProtocolVersion = options.AppProtocolVersion;
        _blocksyncPort = nodeOptions.Value.BlocksyncPort;
        _consensusPort = nodeOptions.Value.ConsensusPort;
        UpdateNodeInfo();
        _logger.LogDebug("Node is created: {Address}", Address);
        _logger.LogDebug(JsonUtility.Serialize(Info));
    }

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public string StorePath => _storePath;

    public bool IsRunning { get; private set; }

    public bool IsDisposed => _isDisposed;

    public PublicKey PublicKey { get; }

    public Address Address => PublicKey.Address;

    public BlockChain BlockChain => _swarm?.BlockChain ?? throw new InvalidOperationException();

    public NodeInfo Info { get; private set; } = NodeInfo.Empty;

    public INodeContent[] Contents
    {
        get => _contents ?? throw new InvalidOperationException("Contents is not initialized.");
        set => _contents = value;
    }

    public Swarm Swarm => _swarm ?? throw new InvalidOperationException();

    public BoundPeer[] Peers
    {
        get
        {
            if (_swarm is not null)
            {
                return [.. _swarm.Peers.Select(item => item)];
            }

            throw new InvalidOperationException();
        }
    }

    public EndPoint? SeedEndPoint
    {
        get => _seedEndPoint;
        set
        {
            if (IsRunning == true)
            {
                throw new InvalidOperationException("The client is running.");
            }

            _seedEndPoint = value;
        }
    }

    public override string ToString() => $"{Address.ToShortString()}";

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

    public byte[] Sign(object obj) => _privateKey.Sign(obj);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        if (IsRunning is true)
        {
            throw new InvalidOperationException("Node is already running.");
        }

        var (blocksyncSeedPeer, consensusSeedPeer)
            = await GetSeedInfoAsync(_logger, cancellationToken);
        var privateKey = _privateKey;
        var appProtocolVersion = _appProtocolVersion;
        var storePath = _storePath;
        var blocksyncPort = _blocksyncPort;
        var consensusPort = _consensusPort;
        var swarmTransport
            = await CreateTransport(privateKey, blocksyncPort, appProtocolVersion);
        var swarmOptions = new SwarmOptions
        {
            StaticPeers = blocksyncSeedPeer is null ? [] : [blocksyncSeedPeer],
            BootstrapOptions = new()
            {
                SeedPeers = blocksyncSeedPeer is null ? [] : [blocksyncSeedPeer],
            },
        };
        var consensusTransport = await CreateTransport(
            privateKey: privateKey,
            port: consensusPort,
            appProtocolVersion: appProtocolVersion);
        var consensusReactorOption = new ConsensusReactorOption
        {
            SeedPeers = consensusSeedPeer is null ? [] : [consensusSeedPeer],
            ConsensusPort = consensusPort,
            ConsensusPrivateKey = privateKey,
            TargetBlockInterval = TimeSpan.FromSeconds(2),
            ContextTimeoutOptions = new(),
        };
        var (keyValueStore, store, stateStore) = BlockChainUtility.GetStore(storePath);
        var blockChain = BlockChainUtility.CreateBlockChain(
            genesisBlock: _genesisBlock,
            store,
            stateStore,
            renderer: this,
            actionProvider: _actionProvider);

        _keyValueStore = keyValueStore;
        _store = store;
        _stateStore = stateStore;
        _swarm = new Swarm(
            blockChain: blockChain,
            privateKey: privateKey,
            transport: swarmTransport,
            options: swarmOptions,
            consensusTransport: consensusTransport,
            consensusOption: consensusReactorOption);
        _startTask = _swarm.StartAsync(cancellationToken: default);
        _logger.LogDebug("Node.Swarm is starting: {Address}", Address);
        if (blocksyncSeedPeer is not null)
        {
            await _swarm.BootstrapAsync(cancellationToken: default);
            _logger.LogDebug("Node.Swarm is bootstrapped: {Address}", Address);
        }

        UpdateNodeInfo();
        _logger.LogDebug(JsonUtility.Serialize(Info));
        IsRunning = true;
        _logger.LogDebug("Node is started: {Address}", Address);
        await Task.WhenAll(Contents.Select(item => item.StartAsync(cancellationToken)));
        _logger.LogDebug("Node Contents are started: {Address}", Address);
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        if (IsRunning is false)
        {
            throw new InvalidOperationException("Node is not running.");
        }

        await Task.WhenAll(Contents.Select(item => item.StopAsync(cancellationToken)));
        _logger.LogDebug("Node Contents are stopped: {Address}", Address);

        if (_swarm is not null)
        {
            await _swarm.StopAsync(cancellationToken: cancellationToken);
            await _startTask;
            _logger.LogDebug("Node.Swarm is stopping: {Address}", Address);
            _swarm.Dispose();
            _logger.LogDebug("Node.Swarm is stopped: {Address}", Address);
        }

        _swarm = null;

        _keyValueStore?.Dispose();
        _keyValueStore = null;
        _store?.Dispose();
        _store = null;
        _stateStore?.Dispose();
        _stateStore = null;
        _startTask = Task.CompletedTask;
        IsRunning = false;
        UpdateNodeInfo();
        _logger.LogDebug("Node is stopped: {Address}", Address);
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed is false)
        {
            if (_swarm is not null)
            {
                await _swarm.StopAsync(cancellationToken: default);
                _swarm.Dispose();
            }

            await (_startTask ?? Task.CompletedTask);
            _startTask = Task.CompletedTask;
            _isDisposed = true;
        }
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

            UpdateNodeInfo();
            BlockAppended?.Invoke(this, new(Info.Tip));
        }
    }

    private static async Task<NetMQTransport> CreateTransport(
        PrivateKey privateKey, int port, AppProtocolVersion appProtocolVersion)
    {
        var appProtocolVersionOptions = new AppProtocolVersionOptions
        {
            AppProtocolVersion = appProtocolVersion,
        };
        var hostOptions = new HostOptions("localhost", [], port);
        return await NetMQTransport.Create(privateKey, appProtocolVersionOptions, hostOptions);
    }

    private async Task<(BoundPeer? BlocksyncPeer, BoundPeer? ConsensusPeer)> GetSeedInfoAsync(
        ILogger logger, CancellationToken cancellationToken)
    {
        if (_seedEndPoint is { } seedEndPoint)
        {
            logger.LogDebug("Getting seed info from {SeedEndPoint}", seedEndPoint);
            var address = $"http://{EndPointUtility.ToString(seedEndPoint)}";
            var channelOptions = new GrpcChannelOptions
            {
            };
            using var channel = GrpcChannel.ForAddress(address, channelOptions);
            var client = new Grpc.Seed.SeedGrpcService.SeedGrpcServiceClient(channel);
            var request = new Grpc.Seed.GetSeedRequest
            {
                PublicKey = new PrivateKey().PublicKey.ToHex(compress: true),
            };

            var response = await client.GetSeedAsync(request, cancellationToken: cancellationToken);
            var seedInfo = (SeedInfo)response.SeedResult;
            logger.LogDebug("Got seed info from {SeedEndPoint}", seedEndPoint);
            return (seedInfo.BlocksyncSeedPeer, seedInfo.ConsensusSeedPeer);
        }

        return (null, null);
    }

    private void UpdateNodeInfo()
    {
        var appProtocolVersion = _appProtocolVersion;
        var nodeInfo = NodeInfo.Empty with
        {
            ProcessId = Environment.ProcessId,
            Address = Address,
            AppProtocolVersion = appProtocolVersion.Token,
            BlocksyncPort = _blocksyncPort,
            ConsensusPort = _consensusPort,
        };

        if (IsRunning == true)
        {
            nodeInfo = nodeInfo with
            {
                GenesisHash = BlockChain.Genesis.Hash,
                Tip = new BlockInfo(BlockChain.Tip),
                IsRunning = IsRunning,
            };
        }

        Info = nodeInfo;
    }
}
