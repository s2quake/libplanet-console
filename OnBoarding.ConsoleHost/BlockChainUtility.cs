using System.Collections.Immutable;
using System.Numerics;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.Loader;
using Libplanet.Action.Sys;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Crypto;
using Libplanet.RocksDBStore;
using Libplanet.Store;
using Libplanet.Types.Blocks;
using Libplanet.Types.Consensus;
using Libplanet.Types.Tx;

namespace OnBoarding.ConsoleHost;

static class BlockChainUtility
{
    public static readonly PrivateKey GenesisProposer = PrivateKey.FromString
    (
        "2a15e7deaac09ce631e1faa184efadb175b6b90989cf1faed9dfc321ad1db5ac"
    );

    public static BlockChain CreateBlockChain(string name, PublicKey[] validatorKeys)
    {
        var dataPath = Path.Combine(Directory.GetCurrentDirectory(), ".data", name);
        var keyValueStore = new RocksDBKeyValueStore(dataPath);
        var stateStore = new TrieStateStore(keyValueStore);
        var actionLoader = TypedActionLoader.Create(typeof(Application).Assembly);
        var store = new MemoryStore();
        var actionEvaluator = new ActionEvaluator(_ => null, stateStore, actionLoader);
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
            actions: new IValue[] { action.PlainValue },
            timestamp: DateTimeOffset.MinValue
            );
        var transactions = ImmutableList.Create(transaction);
        var genesisBlock = BlockChain.ProposeGenesisBlock(actionEvaluator, GenesisProposer, transactions, timestamp: DateTimeOffset.MinValue);
        var policy = new BlockPolicy(
            blockInterval: TimeSpan.FromMilliseconds(1),
            getMaxTransactionsPerBlock: _ => int.MaxValue,
            getMaxTransactionsBytes: _ => long.MaxValue);
        var stagePolicy = new VolatileStagePolicy();
        return BlockChain.Create(policy, stagePolicy, store, stateStore, genesisBlock, actionEvaluator);
    }

    public static void Stage(BlockChain blockChain, User user, IAction[] actions)
    {
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
    }

    public static Block AppendNew(BlockChain blockChain, User user, PrivateKey[] validators, IAction[] actions)
    {
        var block = AppendNew(blockChain, user, validators, actions.Select(item => item.PlainValue).ToArray());
        return block;
    }

    public static Block AppendNew(BlockChain blockChain, User user, PrivateKey[] validators, IValue[] values)
    {
        var privateKey = user.PrivateKey;
        var genesisBlock = blockChain.Genesis;
        var nonce = blockChain.GetNextTxNonce(privateKey.ToAddress());
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
            txHash: BlockContent.DeriveTxHash(new Transaction[] { transaction }),
            lastCommit: lastCommit
        );
        var blockContent = new BlockContent(blockMetadata, new Transaction[] { transaction });
        var preEvaluationBlock = blockContent.Propose();
        var stateRootHash = blockChain.DetermineBlockStateRootHash(preEvaluationBlock, out _);
        var height = blockChain.Count;
        var round = 0;
        var block = preEvaluationBlock.Sign(privateKey, stateRootHash);
        var votes = validators.OrderBy(item => item.ToAddress()).Select(item =>
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
}
