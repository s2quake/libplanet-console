using System.Collections.Immutable;
using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Types.Blocks;
using Libplanet.Types.Consensus;
using Libplanet.Types.Tx;

namespace OnBoarding.ConsoleHost;

static class BlockChainUtils
{
    public static Block AppendNew(BlockChain _blockChain, User user, UserCollection users, IAction[] actions)
    {
        var privateKey = user.PrivateKey;
        var genesisBlock = _blockChain.Genesis;
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
        var votes = users.OrderBy(item => item.Address).Select(item =>
        {
            var voteMetadata = new VoteMetadata(
            height: height,
            round: round,
            blockHash: block.Hash,
            timestamp: DateTimeOffset.UtcNow,
            validatorPublicKey: item.PublicKey,
            flag: VoteFlag.PreCommit);
            return voteMetadata.Sign(item.PrivateKey);
        }).ToImmutableArray();

        var blockCommit = new BlockCommit(height, round, block.Hash, votes);
        _blockChain.Append(block, blockCommit);
        return block;
    }
}
