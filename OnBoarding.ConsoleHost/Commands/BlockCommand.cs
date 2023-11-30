using System.ComponentModel.Composition;
using System.Text;
using JSSoft.Library.Commands;
using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Types.Blocks;
using Libplanet.Types.Consensus;
using Libplanet.Types.Tx;
using OnBoarding.ConsoleHost.Actions;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
sealed class BlockCommand(BlockChain blockChain) : CommandMethodBase
{
    private readonly BlockChain _blockChain = blockChain;

    [CommandMethod]
    public void New(int count = 1)
    {
        for (var i = 0; i < count; i++)
        {
            AddNewBlock();
        }
    }

    [CommandMethod]
    public void List()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < _blockChain.Count; i++)
        {
            var block = _blockChain[i];
            sb.AppendLine($"[{i}]: {block.Hash}");
        }
        Out.Write(sb.ToString());
    }

    private void AddNewBlock()
    {
        var genesisBlock = _blockChain.Genesis;
        var actions = new IAction[]
        {
            new AttackAction(),
            new HealAction(),
        };
        var nonce = _blockChain.GetNextTxNonce(Application.PrivateKey.ToAddress());
        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: Application.PrivateKey,
            genesisHash: genesisBlock.Hash,
            actions: actions.Select(item => item.PlainValue)
        );
        var previousBlock = _blockChain[_blockChain.Count - 1];
        var lastCommit = _blockChain.GetBlockCommit(previousBlock.Hash);
        var blockMetadata = new BlockMetadata(
            index: _blockChain.Count,
            publicKey: Application.PublicKey,
            timestamp: DateTimeOffset.UtcNow,
            previousHash: previousBlock.Hash,
            txHash: BlockContent.DeriveTxHash([transaction]),
            lastCommit: lastCommit
        );
        var blockContent = new BlockContent(blockMetadata, [transaction]);
        var preEvaluationBlock = blockContent.Propose();
        var stateRootHash = _blockChain.DetermineBlockStateRootHash(preEvaluationBlock, out _);
        var height = _blockChain.Count;
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
}
