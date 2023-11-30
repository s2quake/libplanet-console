using System.Net;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Net.Options;
using Libplanet.Net.Transports;
using Libplanet.Net.Consensus;
using Libplanet.Blockchain;

namespace OnBoarding.ConsoleHost;

sealed class SwarmHost(PrivateKey privateKey, BlockChain blockChain) : IAsyncDisposable
{
    private readonly PrivateKey _privateKey = privateKey;
    private readonly BlockChain _blockChain = blockChain;
    private Swarm _swarm = Create(privateKey, blockChain);
    private Task? _startTask;
    private bool _isDisposed;

    public string Key => $"{_privateKey.PublicKey}";

    public bool IsRunning => _swarm != null;

    public bool IsDisposed => _isDisposed;

    public Swarm Target => _swarm;

    public event EventHandler? Disposed;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_startTask != null)
            throw new InvalidOperationException("Swarm has been started.");

        _startTask = _swarm.StartAsync(cancellationToken: default);
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_startTask == null)
            throw new InvalidOperationException("Swarm has been stopped.");

        await _swarm.StopAsync(cancellationToken: cancellationToken);
        await _startTask;
        _swarm.Dispose();
        _startTask = null;
    }

    private static Swarm Create(PrivateKey privateKey, BlockChain blockChain)
    {
        var transport = CreateTransport(privateKey);
        var swarmOptions = new SwarmOptions
        {

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
        return new Swarm(blockChain, privateKey, transport, null, null, consensusOption: consensusReactorOption);
    }

    private static ITransport CreateTransport(PrivateKey privateKey)
    {
        var apv = AppProtocolVersion.Sign(privateKey, 1);
        var appProtocolVersionOptions = new AppProtocolVersionOptions { AppProtocolVersion = apv };
        var hostOptions = new HostOptions($"{IPAddress.Loopback}", Array.Empty<IceServer>());
        var task = NetMQTransport.Create(privateKey, appProtocolVersionOptions, hostOptions);
        task.Wait();
        return task.Result;
    }

    // private static BlockChain CreateBlockChain(PrivateKey privateKey)
    // {
    //     var dataPath = Path.Combine(Directory.GetCurrentDirectory(), ".data");
    //     var blockPolicy = new BlockPolicy();
    //     var stagePolicy = new VolatileStagePolicy();
    //     var store = new MemoryStore();
    //     var keyValueStore = new DefaultKeyValueStore(dataPath);
    //     var stateStore = new TrieStateStore(keyValueStore);
    //     var publicKey = privateKey.PublicKey;
    //     var validatorList = new List<Validator>
    //     {
    //         new(privateKey.PublicKey, BigInteger.One),
    //     };
    //     var validatorSet = new ValidatorSet(validatorList);
    //     var nonce = 0L;
    //     var action = new Initialize(
    //         validatorSet: validatorSet,
    //         states: ImmutableDictionary.Create<Address, IValue>()
    //         );
    //     var transaction = Transaction.Create(
    //         nonce,
    //         privateKey,
    //         genesisHash: null,
    //         actions: [action.PlainValue],
    //         timestamp: DateTimeOffset.MinValue
    //         );
    //     // var transactions = new Transaction[] { transaction };
    //     var blockMetadata = new BlockMetadata(
    //         protocolVersion: Block.CurrentProtocolVersion,
    //         index: 0,
    //         timestamp: DateTimeOffset.Now,
    //         miner: new Address(privateKey.PublicKey),
    //         publicKey,
    //         previousHash: null,
    //         txHash: BlockContent.DeriveTxHash([transaction]),
    //         lastCommit: null);
    //     var blockContent = new BlockContent(blockMetadata, [transaction]);
    //     var preEvaluationBlock = blockContent.Propose();
    //     var actionLoader = TypedActionLoader.Create(typeof(Application).Assembly);
    //     var actionEvaluator = new ActionEvaluator(_ => null, stateStore, actionLoader);
    //     var stateRootHash = BlockChain.DetermineGenesisStateRootHash(
    //         actionEvaluator,
    //         preEvaluationBlock,
    //         out _
    //     );
    //     var genesisBlock = preEvaluationBlock.Sign(privateKey, stateRootHash);
    //     var blockChain = BlockChain.Create(
    //         blockPolicy,
    //         stagePolicy,
    //         store,
    //         stateStore,
    //         genesisBlock: genesisBlock,
    //         actionEvaluator: actionEvaluator
    //     );
    //     return blockChain;
    // }

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
}
