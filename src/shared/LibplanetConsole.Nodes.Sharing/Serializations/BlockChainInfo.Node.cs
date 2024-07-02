#if LIBPLANET_NODE
using Libplanet.Blockchain;

namespace LibplanetConsole.Nodes.Serializations;

public readonly partial record struct BlockChainInfo
{
    public BlockChainInfo(BlockChain blockChain)
    {
        var blocks = new BlockInfo[blockChain.Count];
        for (var i = 0; i < blockChain.Count; i++)
        {
            blocks[i] = new BlockInfo(blockChain[i]);
        }

        GenesisHash = $"{blockChain.Genesis.Hash}";
    }
}
#endif // LIBPLANET_NODE
