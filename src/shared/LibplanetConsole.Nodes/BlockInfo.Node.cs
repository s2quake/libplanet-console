#if LIBPLANET_NODE
using System.Linq;
using Libplanet.Blockchain;
using Libplanet.Types.Blocks;
using Libplanet.Types.Tx;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes;

public readonly partial record struct BlockInfo
{
    public BlockInfo(BlockChain blockChain, Block block)
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
}
#endif // LIBPLANET_NODE
