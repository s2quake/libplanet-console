using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Types.Blocks;
using Libplanet.Types.Consensus;
using Libplanet.Types.Tx;

namespace OnBoarding.ConsoleHost;

static class BlockChainUtils
{
    public static Block AppendNew(BlockChain blockChain, User user, UserCollection users, IAction[] actions)
    {
        var block = AppendNew(blockChain, user, users, actions.Select(item => item.PlainValue).ToArray());
        return block;
    }

    public static Block AppendNew(BlockChain blockChain, User user, UserCollection users, IValue[] values)
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
            txHash: BlockContent.DeriveTxHash([transaction]),
            lastCommit: lastCommit
        );
        var blockContent = new BlockContent(blockMetadata, [transaction]);
        var preEvaluationBlock = blockContent.Propose();
        var stateRootHash = blockChain.DetermineBlockStateRootHash(preEvaluationBlock, out _);
        var height = blockChain.Count;
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
        blockChain.Append(block, blockCommit);
        return block;
    }
}
