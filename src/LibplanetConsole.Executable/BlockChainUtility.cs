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
using Libplanet.RocksDBStore;
using Libplanet.Store;
using Libplanet.Store.Trie;
using Libplanet.Types.Blocks;
using Libplanet.Types.Consensus;
using Libplanet.Types.Tx;
using LibplanetConsole.Executable.BeginActions;
using LibplanetConsole.Executable.EndActions;

namespace LibplanetConsole.Executable;

static class BlockChainUtility
{
    public static readonly PrivateKey GenesisProposer = PrivateKey.FromString
    (
        "2a15e7deaac09ce631e1faa184efadb175b6b90989cf1faed9dfc321ad1db5ac"
    );

    public static BlockChain CreateBlockChain(PublicKey[] validatorKeys, string storePath, IRenderer renderer)
    {
        var isNew = storePath == string.Empty || Directory.Exists(storePath) == false;
        var (store, stateStore) = GetStore(storePath);
        var actionLoader = TypedActionLoader.Create(typeof(Application).Assembly);
        var beginActions = new IAction[]
        {
        };
        var endActions = new IAction[]
        {
        };
        var actionEvaluator = new ActionEvaluator(
            policyBeginBlockActionsGetter: _ => beginActions.ToImmutableArray(),
            policyEndBlockActionsGetter: _ => endActions.ToImmutableArray(),
            stateStore,
            actionLoader
        );
        var validatorList = validatorKeys.Select(item => new Validator(item, BigInteger.One)).ToList();
        var validatorSet = new ValidatorSet(validatorList);
        var nonce = 0L;
        var action = new Initialize(
            validatorSet: validatorSet,
            states: ImmutableDictionary.Create<Address, IValue>()
            );
        var transaction = Transaction.Create(
            nonce,
            GenesisProposer,
            genesisHash: null,
            actions: [action.PlainValue],
            timestamp: DateTimeOffset.MinValue
            );
        var transactions = ImmutableList.Create(transaction);
        var genesisBlock = BlockChain.ProposeGenesisBlock(actionEvaluator, GenesisProposer, transactions, timestamp: DateTimeOffset.MinValue);
        var policy = new BlockPolicy(
            blockInterval: TimeSpan.FromMilliseconds(1),
            getMaxTransactionsPerBlock: _ => int.MaxValue,
            getMaxTransactionsBytes: _ => long.MaxValue);
        var stagePolicy = new VolatileStagePolicy();
        var blockChainStates = new BlockChainStates(store, stateStore);
        var renderers = new IRenderer[] { renderer };
        if (isNew == true)
            return BlockChain.Create(policy, stagePolicy, store, stateStore, genesisBlock, actionEvaluator, renderers, blockChainStates);
        return new BlockChain(policy, stagePolicy, store, stateStore, genesisBlock, blockChainStates, actionEvaluator, renderers);
    }

    [Obsolete("Do not use this method. It exists to help understand how blocks can be appended to the blockchain.")]
    public static Block AppendNew(BlockChain blockChain, Client client, PrivateKey[] validators, IAction[] actions)
    {
        var block = AppendNew(blockChain, client, validators, actions.Select(item => item.PlainValue).ToArray());
        return block;
    }

    [Obsolete("Do not use this method. It exists to help understand how blocks can be appended to the blockchain.")]
    public static Block AppendNew(BlockChain blockChain, Client client, PrivateKey[] validators, IValue[] values)
    {
        var privateKey = client.PrivateKey;
        var genesisBlock = blockChain.Genesis;
        var nonce = blockChain.GetNextTxNonce(privateKey.Address);
        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: genesisBlock.Hash,
            actions: new TxActionList(values)
        );

        var previousBlock = blockChain[blockChain.Count - 1];
        var lastCommit = blockChain.GetBlockCommit(previousBlock.Hash);
        var blockMetadata = new BlockMetadata(
            index: blockChain.Count,
            publicKey: privateKey.PublicKey,
            timestamp: DateTimeOffset.UtcNow,
            previousHash: previousBlock.Hash,
            txHash: BlockContent.DeriveTxHash([transaction]),
            lastCommit: lastCommit
        );
        var blockContent = new BlockContent(blockMetadata, [transaction]);
        var preEvaluationBlock = blockContent.Propose();
        var stateRootHash = blockChain.DetermineBlockStateRootHash(preEvaluationBlock, out _);
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
            flag: VoteFlag.PreCommit);
            return voteMetadata.Sign(item);
        }).ToImmutableArray();

        var blockCommit = new BlockCommit(height, round, block.Hash, votes);
        blockChain.Append(block, blockCommit);
        return block;
    }

    private static (IStore store, IStateStore stateStore) GetStore(string storePath)
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
