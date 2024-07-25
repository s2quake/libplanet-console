#if LIBPLANET_NODE
using Libplanet.Blockchain;
#endif // LIBPLANET_NODE
using Libplanet.Types.Blocks;
using Libplanet.Types.Tx;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes;

public readonly record struct BlockInfo
{
    public BlockInfo()
    {
    }

#if LIBPLANET_NODE
    internal BlockInfo(BlockChain blockChain, Block block)
    {
        Height = block.Index;
        Hash = (AppHash)block.Hash;
        Miner = (AppAddress)block.Miner;
        Transactions = [.. block.Transactions.Select(GetTransaction)];

        TransactionInfo GetTransaction(Transaction transaction)
        {
            var execution = blockChain.GetTxExecution(block.Hash, transaction.Id);
            return new TransactionInfo(execution, transaction) { Height = block.Index };
        }
    }
#endif // LIBPLANET_NODE

    public long Height { get; init; }

    public AppHash Hash { get; init; }

    public AppAddress Miner { get; init; }

    public TransactionInfo[] Transactions { get; init; } = [];
}
