using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Numerics;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.Loader;
using Libplanet.Action.Sys;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Crypto;
using Libplanet.Store;
using Libplanet.Store.Trie;
using Libplanet.Types.Consensus;
using Libplanet.Types.Tx;

namespace OnBoarding.ConsoleHost;

sealed partial class Application
{
    private void InitializeService()
    {
        if (GetService<UserCollection>() is { } users)
        {
            var privateKey = users.First().PrivateKey;
            var stateStore = CreateStateStore();
            var actionLoader = TypedActionLoader.Create(typeof(Application).Assembly);
            var store = new MemoryStore();
            var actionEvaluator = new ActionEvaluator(_ => null, stateStore, actionLoader);
            var validatorList = users.Select(item => new Validator(item.PublicKey, BigInteger.One)).ToList();
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
            var genesisBlock = BlockChain.ProposeGenesisBlock(actionEvaluator, privateKey, [transaction]);

            var policy = new BlockPolicy(
                blockInterval: TimeSpan.FromMilliseconds(1),
                getMaxTransactionsPerBlock: _ => int.MaxValue,
                getMaxTransactionsBytes: _ => long.MaxValue);
            var stagePolicy = new VolatileStagePolicy();
            var blockChain = BlockChain.Create(policy, stagePolicy, store, stateStore, genesisBlock, actionEvaluator);

            _container.ComposeExportedValue(AttributedModelServices.GetContractName(typeof(IStateStore)), stateStore);
            _container.ComposeExportedValue<IActionLoader>(actionLoader);
            _container.ComposeExportedValue<IStore>(store);
            _container.ComposeExportedValue<BlockChain>(blockChain);
        }
    }

    private static IStateStore CreateStateStore()
    {
        var dataPath = Path.Combine(Directory.GetCurrentDirectory(), ".data");
        var defaultKeyValueStore = new DefaultKeyValueStore(dataPath);
        return new TrieStateStore(defaultKeyValueStore);
    }
}
