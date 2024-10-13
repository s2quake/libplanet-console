using LibplanetConsole.Blockchain.Grpc;

namespace LibplanetConsole.Blockchain;

public readonly partial record struct BlockInfo
{
    public BlockInfo()
    {
    }

    public long Height { get; init; }

    public BlockHash Hash { get; init; }

    public Address Miner { get; init; }

    public static BlockInfo Empty { get; } = new BlockInfo
    {
        Height = -1,
    };

    public static implicit operator BlockInfo(BlockInformation blockInfo)
    {
        return new BlockInfo
        {
            Height = blockInfo.Height,
            Hash = BlockHash.FromString(blockInfo.Hash),
            Miner = new Address(blockInfo.Miner),
        };
    }

    public static implicit operator BlockInformation(BlockInfo blockInfo)
    {
        return new BlockInformation
        {
            Height = blockInfo.Height,
            Hash = blockInfo.Hash.ToString(),
            Miner = blockInfo.Miner.ToHex(),
        };
    }
}
