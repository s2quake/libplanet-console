using GrpcBlockInfo = LibplanetConsole.Node.Grpc.BlockInfo;

namespace LibplanetConsole.Node;

public readonly partial record struct BlockInfo
{
    public BlockInfo()
    {
    }

    public long Height { get; init; }

    public BlockHash Hash { get; init; }

    public Address Miner { get; init; }

    public static implicit operator BlockInfo(GrpcBlockInfo blockInfo)
    {
        return new BlockInfo
        {
            Height = blockInfo.Height,
            Hash = BlockHash.FromString(blockInfo.Hash),
            Miner = new Address(blockInfo.Miner),
        };
    }

    public static implicit operator GrpcBlockInfo(BlockInfo blockInfo)
    {
        return new GrpcBlockInfo
        {
            Height = blockInfo.Height,
            Hash = blockInfo.Hash.ToString(),
            Miner = blockInfo.Miner.ToHex(),
        };
    }
}
