using System.Collections.Immutable;
using System.Numerics;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.Loader;
using Libplanet.Action.Sys;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Blockchain.Renderers;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.RocksDBStore;
using Libplanet.Store;
using Libplanet.Store.Trie;
using Libplanet.Types.Blocks;
using Libplanet.Types.Consensus;
using Libplanet.Types.Tx;
using LibplanetConsole.Common.Actions;

namespace LibplanetConsole.Common;

public static class BlockChainUtility
{
    public static readonly PrivateKey AppProtocolKey
        = PrivateKey.FromString("2a15e7deaac09ce631e1faa184efadb175b6b90989cf1faed9dfc321ad1db5ac");

    public static readonly AppProtocolVersion AppProtocolVersion
        = AppProtocolVersion.Sign(AppProtocolKey, 1);

    public static BlockChain CreateBlockChain(
        GenesisOptions genesisOptions,
        string storePath,
        IRenderer renderer,
        IActionLoader[] actionLoaders)
    {
        var genesisKey = (PrivateKey)genesisOptions.GenesisKey;
        var isNew = storePath == string.Empty || Directory.Exists(storePath) != true;
        var (store, stateStore) = GetStore(storePath);
        var actionLoader = new AggregateTypedActionLoader(actionLoaders);
        var actionEvaluator = new ActionEvaluator(
            policyActionsRegistry: new(),
            stateStore,
            actionLoader);
        var validators = genesisOptions.GenesisValidators
                            .Select(item => new Validator((PublicKey)item, new BigInteger(1000)))
                            .ToArray();
        var validatorSet = new ValidatorSet(validators: [.. validators]);
        var nonce = 0L;
        IAction[] actions =
        [
            new Initialize(
                validatorSet: validatorSet,
                states: ImmutableDictionary.Create<Address, IValue>()),
        ];

        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: genesisKey,
            genesisHash: null,
            actions: [.. actions.Select(item => item.PlainValue)],
            timestamp: DateTimeOffset.MinValue);
        var transactions = ImmutableList.Create(transaction);
        var genesisBlock = BlockChain.ProposeGenesisBlock(
            privateKey: genesisKey,
            transactions: transactions,
            timestamp: DateTimeOffset.MinValue);
        var policy = new BlockPolicy(
            blockInterval: TimeSpan.FromSeconds(10),
            getMaxTransactionsPerBlock: _ => int.MaxValue,
            getMaxTransactionsBytes: _ => long.MaxValue);
        var stagePolicy = new VolatileStagePolicy();
        var blockChainStates = new BlockChainStates(store, stateStore);
        var renderers = new IRenderer[] { renderer };
        if (isNew == true)
        {
            return BlockChain.Create(
                policy: policy,
                stagePolicy: stagePolicy,
                store: store,
                stateStore: stateStore,
                genesisBlock: genesisBlock,
                actionEvaluator: actionEvaluator,
                renderers: renderers,
                blockChainStates: blockChainStates);
        }

        return new BlockChain(
            policy: policy,
            stagePolicy: stagePolicy,
            store: store,
            stateStore: stateStore,
            genesisBlock: genesisBlock,
            blockChainStates: blockChainStates,
            actionEvaluator: actionEvaluator,
            renderers: renderers);
    }

    [Obsolete("""
    Do not use this method.
    It exists to help understand how blocks can be appended to the blockchain.
    """)]
    public static Block AppendNew(
        BlockChain blockChain, PrivateKey privateKey, PrivateKey[] validators, IAction[] actions)
    {
        var values = actions.Select(item => item.PlainValue).ToArray();
        var block = AppendNew(blockChain, privateKey, validators, values);
        return block;
    }

    [Obsolete("""
    Do not use this method.
    It exists to help understand how blocks can be appended to the blockchain.
    """)]
    public static Block AppendNew(
        BlockChain blockChain, PrivateKey privateKey, PrivateKey[] validators, IValue[] values)
    {
        var genesisBlock = blockChain.Genesis;
        var nonce = blockChain.GetNextTxNonce(privateKey.Address);
        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: genesisBlock.Hash,
            actions: new TxActionList(values));

        var previousBlock = blockChain[blockChain.Count - 1];
        var lastCommit = blockChain.GetBlockCommit(previousBlock.Hash);
        var blockMetadata = new BlockMetadata(
            index: blockChain.Count,
            timestamp: DateTimeOffset.UtcNow,
            publicKey: privateKey.PublicKey,
            previousHash: previousBlock.Hash,
            txHash: BlockContent.DeriveTxHash([transaction]),
            lastCommit: lastCommit,
            evidenceHash: null);
        var blockContent = new BlockContent(blockMetadata, [transaction], []);
        var preEvaluationBlock = blockContent.Propose();
        var stateRootHash = blockChain.DetermineNextBlockStateRootHash(blockChain.Tip, out _);
        var height = blockChain.Count;
        var round = 0;
        var block = preEvaluationBlock.Sign(privateKey, stateRootHash);
        var votes = validators.OrderBy(item => item.Address).Select(item =>
        {
            var voteMetadata = new VoteMetadata(
            height: height,
            round: round,
            blockHash: block.Hash,
            timestamp: DateTimeOffset.UtcNow,
            validatorPublicKey: item.PublicKey,
            validatorPower: BigInteger.One,
            flag: VoteFlag.PreCommit);
            return voteMetadata.Sign(item);
        }).ToImmutableArray();

        var blockCommit = new BlockCommit(height, round, block.Hash, votes);
        blockChain.Append(block, blockCommit);
        return block;
    }

    private static (IStore Store, IStateStore StateStore) GetStore(string storePath)
    {
        if (storePath != string.Empty)
        {
            var dataPath1 = Path.Combine(storePath, "state");
            var dataPath2 = Path.Combine(storePath, "store");
            var keyValueStore = new RocksDBKeyValueStore(dataPath1);
            var stateStore = new TrieStateStore(keyValueStore);
            var store = new RocksDBStore(dataPath2);
            return (store, stateStore);
        }
        else
        {
            var keyValueStore = new MemoryKeyValueStore();
            var stateStore = new TrieStateStore(keyValueStore);
            var store = new MemoryStore();
            return (store, stateStore);
        }
    }
}
