using System.Collections.Concurrent;
using System.Security;
using System.Security.Cryptography;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Renderers;
using Libplanet.Net;
using Libplanet.Net.Consensus;
using Libplanet.Net.Options;
using Libplanet.Net.Transports;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Seed;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LibplanetConsole.Node;

internal sealed partial class Node : IActionRenderer, INode
{
    private readonly SecureString _privateKey;
    private readonly string _storePath;
    private readonly SynchronizationContext _synchronizationContext
        = SynchronizationContext.Current!;

    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<TxId, ManualResetEvent> _eventByTxId = [];
    private readonly ConcurrentDictionary<IValue, Exception> _exceptionByAction = [];
    private readonly ILogger _logger;
    private readonly byte[] _genesis;
    private readonly AppProtocolVersion _appProtocolVersion = SeedNode.AppProtocolVersion;
    private readonly IActionProvider _actionProvider;

    private EndPoint? _seedEndPoint;
    private EndPoint? _blocksyncEndPoint;
    private EndPoint? _consensusEndPoint;
    private Swarm? _swarm;
    private Task _startTask = Task.CompletedTask;
    private bool _isDisposed;

    public Node(IServiceProvider serviceProvider, ApplicationOptions options)
    {
        _serviceProvider = serviceProvider;
        _seedEndPoint = options.SeedEndPoint;
        _privateKey = options.PrivateKey.ToSecureString();
        _storePath = options.StorePath;
        PublicKey = options.PrivateKey.PublicKey;
        _actionProvider = options.ActionProvider ?? ActionProvider.Default;
        _logger = serviceProvider.GetRequiredService<ILogger>();
        _genesis = options.Genesis;
        UpdateNodeInfo();
        _logger.Debug("Node is created: {Address}", Address);
    }

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public EndPoint SwarmEndPoint
    {
        get => _blocksyncEndPoint ?? throw new InvalidOperationException();
        set => _blocksyncEndPoint = value;
    }

    public EndPoint ConsensusEndPoint
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

    public EndPoint SeedEndPoint
    {
        get => _seedEndPoint ??
            throw new InvalidOperationException($"{nameof(SeedEndPoint)} is not initialized.");
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

    public byte[] Sign(object obj) => PrivateKeyUtility.FromSecureString(_privateKey).Sign(obj);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(IsRunning == true, "Node is already running.");

        if (_seedEndPoint is null)
        {
            throw new InvalidOperationException($"{nameof(SeedEndPoint)} is not initialized.");
        }

        var seedInfo = await GetSeedInfoAsync(_seedEndPoint, cancellationToken);
        var privateKey = PrivateKeyUtility.FromSecureString(_privateKey);
        var appProtocolVersion = _appProtocolVersion;
        var storePath = _storePath;
        var blocksyncEndPoint = _blocksyncEndPoint ?? EndPointUtility.NextEndPoint();
        var consensusEndPoint = _consensusEndPoint ?? EndPointUtility.NextEndPoint();
        var blocksyncSeedPeer = seedInfo.BlocksyncSeedPeer;
        var consensusSeedPeer = seedInfo.ConsensusSeedPeer;
        var swarmTransport
            = await CreateTransport(privateKey, blocksyncEndPoint, appProtocolVersion);
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
            appProtocolVersion: appProtocolVersion);
        var consensusReactorOption = new ConsensusReactorOption
        {
            SeedPeers = [consensusSeedPeer],
            ConsensusPort = EndPointUtility.GetHostAndPort(consensusEndPoint).Port,
            ConsensusPrivateKey = privateKey,
            TargetBlockInterval = TimeSpan.FromSeconds(2),
            ContextTimeoutOptions = new(),
        };
        var blockChain = BlockChainUtility.CreateBlockChain(
            genesisBlock: BlockUtility.DeserializeBlock(_genesis),
            storePath: storePath,
            renderer: this,
            actionProvider: _actionProvider);

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
        if (_isDisposed is false)
        {
            _blocksyncEndPoint = null;
            _consensusEndPoint = null;

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

            var blockChain = _swarm!.BlockChain;
            var blockInfo = new BlockInfo(blockChain, blockChain.Tip);
            UpdateNodeInfo();
            BlockAppended?.Invoke(this, new(blockInfo));
        }
    }

    private static async Task<NetMQTransport> CreateTransport(
        PrivateKey privateKey, EndPoint endPoint, AppProtocolVersion appProtocolVersion)
    {
        var appProtocolVersionOptions = new AppProtocolVersionOptions
        {
            AppProtocolVersion = appProtocolVersion,
        };
        var (host, port) = EndPointUtility.GetHostAndPort(endPoint);
        var hostOptions = new HostOptions(host, [], port);
        return await NetMQTransport.Create(privateKey, appProtocolVersionOptions, hostOptions);
    }

    private static async Task<SeedInfo> GetSeedInfoAsync(
        EndPoint seedEndPoint, CancellationToken cancellationToken)
    {
        var remoteService = new RemoteService<ISeedService>();
        var remoteServiceContext = new RemoteServiceContext([remoteService])
        {
            EndPoint = seedEndPoint,
        };
        var closeToken = await remoteServiceContext.OpenAsync(cancellationToken);
        var service = remoteService.Service;
        var privateKey = new PrivateKey();
        var publicKey = privateKey.PublicKey;
        try
        {
            for (var i = 0; i < 10; i++)
            {
                var seedInfo = await service.GetSeedAsync(publicKey, cancellationToken);
                if (Equals(seedInfo, SeedInfo.Empty) != true)
                {
                    return seedInfo;
                }

                await Task.Delay(500, cancellationToken);
            }

            throw new InvalidOperationException("No seed information is available.");
        }
        finally
        {
            await remoteServiceContext.CloseAsync(closeToken, cancellationToken);
        }
    }

    private void UpdateNodeInfo()
    {
        var appProtocolVersion = _appProtocolVersion;
        var nodeInfo = new NodeInfo
        {
            ProcessId = Environment.ProcessId,
            Address = Address,
            AppProtocolVersion = $"{appProtocolVersion}",
        };

        if (IsRunning == true)
        {
            nodeInfo = nodeInfo with
            {
                SwarmEndPoint = EndPointUtility.ToString(SwarmEndPoint),
                ConsensusEndPoint = EndPointUtility.ToString(ConsensusEndPoint),
                GenesisHash = BlockChain.Genesis.Hash,
                TipHash = BlockChain.Tip.Hash,
                IsRunning = IsRunning,
            };
        }

        Info = nodeInfo;
    }
}
