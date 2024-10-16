#if LIBPLANET_NODE
using Libplanet.Blockchain;
using Libplanet.Types.Blocks;
using Libplanet.Types.Tx;
using LibplanetConsole.Common;

namespace LibplanetConsole.Node;

public readonly partial record struct BlockInfo
{
    public BlockInfo(Block block)
    {
        Height = block.Index;
        Hash = block.Hash;
        Miner = block.Miner;
    }
}
#endif // LIBPLANET_NODE
