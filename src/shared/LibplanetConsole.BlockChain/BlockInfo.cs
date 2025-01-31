using LibplanetConsole.BlockChain.Grpc;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.BlockChain;

public readonly partial record struct BlockInfo(long Height, BlockHash Hash, Address Miner)
{
    public static BlockInfo Empty { get; } = new BlockInfo
    {
        Height = -1,
    };

    public static implicit operator BlockInfo(BlockInfoProto blockInfo) => new()
    {
        Height = blockInfo.Height,
        Hash = ToBlockHash(blockInfo.Hash),
        Miner = ToAddress(blockInfo.Miner),
    };

    public static implicit operator BlockInfoProto(BlockInfo blockInfo) => new()
    {
        Height = blockInfo.Height,
        Hash = ToGrpc(blockInfo.Hash),
        Miner = ToGrpc(blockInfo.Miner),
    };
}
