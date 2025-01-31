using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Blockchain.Renderers;
using Libplanet.RocksDBStore;
using Libplanet.Store;
using Libplanet.Store.Trie;

namespace LibplanetConsole.Node;

internal static class BlockChainUtility
{
    public static Libplanet.Blockchain.BlockChain CreateBlockChain(
        Block genesisBlock,
        IStore store,
        IStateStore stateStore,
        IRenderer renderer,
        IActionProvider actionProvider)
    {
        var actionLoader = actionProvider.GetActionLoader();
        var policyActionsRegistry = new PolicyActionsRegistry(
            beginBlockActions: actionProvider.BeginBlockActions,
            endBlockActions: actionProvider.EndBlockActions,
            beginTxActions: actionProvider.BeginTxActions,
            endTxActions: actionProvider.EndTxActions);
        var actionEvaluator = new ActionEvaluator(
            policyActionsRegistry: policyActionsRegistry,
            stateStore,
            actionLoader);
        var policy = new BlockPolicy(
            blockInterval: TimeSpan.FromSeconds(10),
            getMaxTransactionsPerBlock: _ => int.MaxValue,
            getMaxTransactionsBytes: _ => long.MaxValue);
        var stagePolicy = new VolatileStagePolicy();
        var blockChainStates = new BlockChainStates(store, stateStore);
        var renderers = new IRenderer[] { renderer };
        if (store.GetCanonicalChainId() is null)
        {
            return Libplanet.Blockchain.BlockChain.Create(
                policy: policy,
                stagePolicy: stagePolicy,
                store: store,
                stateStore: stateStore,
                genesisBlock: genesisBlock,
                actionEvaluator: actionEvaluator,
                renderers: renderers,
                blockChainStates: blockChainStates);
        }

        return new Libplanet.Blockchain.BlockChain(
            policy: policy,
            stagePolicy: stagePolicy,
            store: store,
            stateStore: stateStore,
            genesisBlock: genesisBlock,
            blockChainStates: blockChainStates,
            actionEvaluator: actionEvaluator,
            renderers: renderers);
    }

    public static (IKeyValueStore KeyValueStore, IStore Store, IStateStore StateStore)
        GetStore(string storePath)
    {
        if (storePath != string.Empty)
        {
            var dataPath1 = Path.Combine(storePath, "state");
            var dataPath2 = Path.Combine(storePath, "store");
            var keyValueStore = new RocksDBKeyValueStore(dataPath1);
            var stateStore = new TrieStateStore(keyValueStore);
            var store = new RocksDBStore(dataPath2);
            return (keyValueStore, store, stateStore);
        }
        else
        {
            var keyValueStore = new MemoryKeyValueStore();
            var stateStore = new TrieStateStore(keyValueStore);
            var store = new MemoryStore();
            return (keyValueStore, store, stateStore);
        }
    }
}
