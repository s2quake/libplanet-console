using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Numerics;
using System.Text;
using Bencodex.Types;
using JSSoft.Library.Commands;
using Libplanet.Action;
using Libplanet.Action.Loader;
using Libplanet.Action.State;
using Libplanet.Action.Sys;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Crypto;
using Libplanet.Net.Consensus;
using Libplanet.Store;
using Libplanet.Types.Blocks;
using Libplanet.Types.Consensus;
using Libplanet.Types.Tx;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnBoarding.ConsoleHost.Actions;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
sealed class BlockCommand(BlockChain blockChain, IStateStore stateStore, IActionLoader actionLoader) : CommandMethodBase
{
    private readonly BlockChain _blockChain = blockChain;

    [CommandMethod]
    public void New()
    {
        var genesisBlock = _blockChain.Genesis;
        var actions = new IAction[]
        {
            new AttackAction(),
            new HealAction(),
        };
        var transaction = Transaction.Create(
            nonce: 1,
            privateKey: Application.PrivateKey,
            genesisHash: genesisBlock.Hash,
            actions: actions.Select(item => item.PlainValue)
        );
        var previousBlock = _blockChain[_blockChain.Count - 1];
        var blockMetadata = new BlockMetadata(
            index: _blockChain.Count,
            publicKey: Application.PublicKey,
            timestamp: DateTimeOffset.UtcNow,
            previousHash: previousBlock.Hash,
            txHash: BlockContent.DeriveTxHash([transaction]),
            lastCommit: null
        );
        var blockContent = new BlockContent(blockMetadata, [transaction]);
        var preEvaluationBlock = blockContent.Propose();
        var stateRootHash = _blockChain.DetermineBlockStateRootHash(preEvaluationBlock, out _);
        var height = 1L;
        var round = 0;
        var block = preEvaluationBlock.Sign(Application.PrivateKey, stateRootHash);

        var voteMetadata = new VoteMetadata(
            height: height,
            round: round,
            blockHash: block.Hash,
            timestamp: DateTimeOffset.UtcNow,
            validatorPublicKey: Application.PublicKey,
            flag: VoteFlag.PreCommit);
        var vote = voteMetadata.Sign(Application.PrivateKey);

        var blockCommit = new BlockCommit(height, round, block.Hash, [vote]);
        _blockChain.Append(block, blockCommit);
        Out.WriteLine(_blockChain.Count);
    }

    [CommandMethod]
    public void List()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < _blockChain.Count; i++)
        {
            var block = _blockChain[i];
            sb.Append($"[{i}]: {block.StateRootHash}");
        }
        Out.Write(sb.ToString());
    }
}
