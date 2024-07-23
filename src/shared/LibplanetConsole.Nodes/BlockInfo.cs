using Libplanet.Blockchain;
using Libplanet.Types.Blocks;
using Libplanet.Types.Tx;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes;

public readonly record struct BlockInfo
{
    public BlockInfo()
    {
    }

    internal BlockInfo(BlockChain blockChain, Block block)
    {
        Index = block.Index;
        Hash = (AppHash)block.Hash;
        Miner = (AppAddress)block.Miner;
        Transactions = [.. block.Transactions.Select(GetTransaction)];

        TransactionInfo GetTransaction(Transaction transaction)
        {
            var execution = blockChain.GetTxExecution(block.Hash, transaction.Id);
            return new TransactionInfo(execution, transaction);
        }
    }

    public long Index { get; init; }

    public AppHash Hash { get; init; }

    public AppAddress Miner { get; init; }

    public TransactionInfo[] Transactions { get; init; } = [];
}
