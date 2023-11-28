
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Libplanet.Types.Tx;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Common;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Net.Options;
using Libplanet.Net.Transports;
using Libplanet.Store;
using Libplanet.Store.Trie;
using Libplanet.Types.Blocks;
using Libplanet.Action;
using Libplanet.Action.Loader;
using Libplanet.Types.Consensus;
using System.Numerics;
using Libplanet.Action.Sys;
using System.Collections.Immutable;
using Bencodex.Types;

namespace OnBoarding.ConsoleHost;

sealed class SwarmFactory
{
    public static async Task<Swarm> CreateAsync()
    {
        var privateKey = new PrivateKey();
        var blockChain = CreateBlockChain(privateKey);
        var transport = await CreateTransportAsync(privateKey);
        return new Swarm(blockChain, privateKey, transport);
    }


    // private static async Task Main(string[] args)
    // {
    //     var privateKey = new PrivateKey();
    //     var blockChain = CreateBlockChain(privateKey);
    //     var transport = await CreateTransportAsync(privateKey);
    //     var swarm = new Swarm(blockChain, privateKey, transport);
    //     var task = swarm.StartAsync();
    //     Console.WriteLine("Press any key to quit.");
    //     Console.ReadKey();
    //     Console.Write("Stopping swarm.");
    //     await swarm.StopAsync();
    //     await task;
    //     Console.Write("Stopped.");
    // }

    private static async Task<ITransport> CreateTransportAsync(PrivateKey privateKey)
    {
        var apv = AppProtocolVersion.Sign(privateKey, 1);
        var appProtocolVersionOptions = new AppProtocolVersionOptions { AppProtocolVersion = apv };
        var hostOptions = new HostOptions($"{IPAddress.Loopback}", Array.Empty<IceServer>());
        var transport = await NetMQTransport.Create(privateKey, appProtocolVersionOptions, hostOptions);
        return transport;
    }

    private static BlockChain CreateBlockChain(PrivateKey privateKey)
    {
        var dataPath = Path.Combine(Directory.GetCurrentDirectory(), ".data");
        var blockPolicy = new BlockPolicy();
        var stagePolicy = new VolatileStagePolicy();
        var store = new MemoryStore();
        var keyValueStore = new DefaultKeyValueStore(dataPath);
        var stateStore = new TrieStateStore(keyValueStore);
        var bytes = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
        var blockHash = new BlockHash(bytes);
        var publicKey = privateKey.PublicKey;
        var hashDigest = new HashDigest<SHA256>(hashDigest: bytes);
        var validatorList = new List<Validator>
            {
                new(privateKey.PublicKey, BigInteger.One),
            };
        var validatorSet = new ValidatorSet(validatorList);
        var nonce = 0L;
        var action = new Initialize(
            validatorSet: validatorSet,
            states: ImmutableDictionary.Create<Address, IValue>()
            );
        var transaction = Transaction.Create(
            nonce,
            privateKey,
            genesisHash: null,
            actions: [action.PlainValue],
            timestamp: DateTimeOffset.MinValue
            );
        var transactions = new Transaction[] { transaction };
        var blockMetadata = new BlockMetadata(
            protocolVersion: Block.CurrentProtocolVersion,
            index: 0,
            timestamp: DateTimeOffset.Now,
            miner: new Address(privateKey.PublicKey),
            publicKey,
            previousHash: null,
            txHash: BlockContent.DeriveTxHash([transaction]),
            lastCommit: null);
        var blockContent = new BlockContent(blockMetadata, [transaction]);
        var preEvaluationBlock = blockContent.Propose();
        var actionEvaluator = new ActionEvaluator(_ => null, stateStore, new SingleActionLoader(typeof(DummyAction)));
        var stateRootHash = BlockChain.DetermineGenesisStateRootHash(
            actionEvaluator,
            preEvaluationBlock,
            out _
        );
        var genesisBlock = preEvaluationBlock.Sign(privateKey, stateRootHash);
        var actionLoader = new SingleActionLoader(typeof(DummyAction));
        var blockChain = BlockChain.Create(
            blockPolicy,
            stagePolicy,
            store,
            stateStore,
            genesisBlock: genesisBlock,
            actionEvaluator: actionEvaluator
        );
        return blockChain;
    }
}
