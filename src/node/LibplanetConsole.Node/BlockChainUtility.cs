using Libplanet.Action;
using Libplanet.Action.Loader;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Blockchain.Renderers;
using Libplanet.RocksDBStore;
using Libplanet.Store;
using Libplanet.Store.Trie;
using Libplanet.Types.Blocks;
using LibplanetConsole.Common.Actions;

namespace LibplanetConsole.Node;

internal static class BlockChainUtility
{
    public static BlockChain CreateBlockChain(
        Block genesisBlock,
        string storePath,
        IRenderer renderer,
        IActionLoader[] actionLoaders)
    {
        var isNew = storePath == string.Empty || Directory.Exists(storePath) != true;
        var (store, stateStore) = GetStore(storePath);
        var actionLoader = new AggregateTypedActionLoader(actionLoaders);
        var actionEvaluator = new ActionEvaluator(
            policyActionsRegistry: new(),
            stateStore,
            actionLoader);
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
