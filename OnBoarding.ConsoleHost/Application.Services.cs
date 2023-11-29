using System.ComponentModel.Composition;
using Libplanet.Action;
using Libplanet.Action.Loader;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Store;
using Libplanet.Store.Trie;

namespace OnBoarding.ConsoleHost;

sealed partial class Application
{
    private IEnumerable<(Type, object)> GetApplicationServices()
    {
        yield return (typeof(IStateStore), CreateStateStore());
        // yield return (typeof(IActionLoader), TypedActionLoader.Create(typeof(Application).Assembly));
        yield return (typeof(IActionLoader), new SingleActionLoader(typeof(DummyAction)));
    }

    private void InitializeService()
    {
        var stateStore = CreateStateStore();
        var actionLoader = new SingleActionLoader(typeof(DummyAction));
        var store = new MemoryStore();
        var actionEvaluator = new ActionEvaluator(_ => null, stateStore, actionLoader);
        var genesisBlock = BlockChain.ProposeGenesisBlock(actionEvaluator);

        var policy = new BlockPolicy(
            blockInterval: TimeSpan.FromMilliseconds(1),
            getMaxTransactionsPerBlock: _ => int.MaxValue,
            getMaxTransactionsBytes: _ => long.MaxValue);
        var stagePolicy = new VolatileStagePolicy();
        var blockChain = BlockChain.Create(policy, stagePolicy, store, stateStore, genesisBlock, actionEvaluator);

        _container.ComposeExportedValue<IStateStore>(stateStore);
        _container.ComposeExportedValue<IActionLoader>(actionLoader);
        _container.ComposeExportedValue<IStore>(store);
        _container.ComposeExportedValue<BlockChain>(blockChain);
    }

    private static IStateStore CreateStateStore()
    {
        var dataPath = Path.Combine(Directory.GetCurrentDirectory(), ".data");
        var defaultKeyValueStore = new DefaultKeyValueStore(dataPath);
        return new TrieStateStore(defaultKeyValueStore);
    }
}
