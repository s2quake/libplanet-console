#if LIBPLANET_NODE
using Libplanet.Types.Blocks;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes.Serializations;

public readonly partial record struct BlockInfo
{
    internal BlockInfo(Block block)
    {
        Index = block.Index;
        Hash = (AppHash)block.Hash;
        Miner = (AppAddress)block.Miner;
        Transactions = block.Transactions.Select(item => new TransactionInfo(item)).ToArray();
    }
}
#endif // LIBPLANET_NODE
