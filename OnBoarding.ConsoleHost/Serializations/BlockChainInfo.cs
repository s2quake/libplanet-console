using Libplanet.Blockchain;

namespace OnBoarding.ConsoleHost.Serializations;

record class BlockChainInfo
{
    public BlockChainInfo(BlockChain blockChain)
    {
        var blocks = new BlockInfo[blockChain.Count];
        for (var i = 0; i < blockChain.Count; i++)
        {
            blocks[i] = new BlockInfo(blockChain[i]);
        }
        Blocks = blocks;
    }

    public BlockInfo[] Blocks { get; }
}
