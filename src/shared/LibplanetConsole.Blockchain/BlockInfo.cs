using LibplanetConsole.Blockchain.Grpc;
using static LibplanetConsole.Blockchain.Grpc.TypeUtility;

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

    public static implicit operator BlockInfo(BlockInfoProto blockInfo)
    {
        return new BlockInfo
        {
            Height = blockInfo.Height,
            Hash = ToBlockHash(blockInfo.Hash),
            Miner = ToAddress(blockInfo.Miner),
        };
    }

    public static implicit operator BlockInfoProto(BlockInfo blockInfo)
    {
        return new BlockInfoProto
        {
            Height = blockInfo.Height,
            Hash = ToGrpc(blockInfo.Hash),
            Miner = ToGrpc(blockInfo.Miner),
        };
    }
}
