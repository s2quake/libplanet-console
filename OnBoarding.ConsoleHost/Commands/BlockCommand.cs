using System.ComponentModel.Composition;
using System.Diagnostics;
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
sealed class BlockCommand(Application application, BlockChain blockChain) : CommandMethodBase
{
    private readonly Application _application = application;
    private readonly BlockChain _blockChain = blockChain;

    [CommandMethod]
    public void New(int count = 1)
    {
        // for (var i = 0; i < count; i++)
        {
            AddNewBlock(count);
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

    private void AddNewBlock(int v)
    {
        if (_application.GetService<UserCollection>() is { } users)
        {
            var privateKey = users.First().PrivateKey;
            var genesisBlock = _blockChain.Genesis;
            var actions = new IAction[]
            {
                new AddAction() { Value = v ,Address = privateKey.ToAddress() },
            };
            var nonce = _blockChain.GetNextTxNonce(privateKey.ToAddress());
            var transaction = Transaction.Create(
                nonce: nonce,
                privateKey: privateKey,
                genesisHash: genesisBlock.Hash,
                actions: actions.Select(item => item.PlainValue)
            );
            var previousBlock = _blockChain[_blockChain.Count - 1];
            var lastCommit = _blockChain.GetBlockCommit(previousBlock.Hash);
            var blockMetadata = new BlockMetadata(
                index: _blockChain.Count,
                publicKey: privateKey.PublicKey,
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
            var block = preEvaluationBlock.Sign(privateKey, stateRootHash);
            var voteMetadata = new VoteMetadata(
                height: height,
                round: round,
                blockHash: block.Hash,
                timestamp: DateTimeOffset.UtcNow,
                validatorPublicKey: privateKey.PublicKey,
                flag: VoteFlag.PreCommit);
            var vote = voteMetadata.Sign(privateKey);
            var blockCommit = new BlockCommit(height, round, block.Hash, [vote]);
            _blockChain.Append(block, blockCommit);
            Out.WriteLine($"Block index #{blockMetadata.Index}: {block.Hash}");
        }
        else
        {
            throw new UnreachableException();
        }
    }
}
