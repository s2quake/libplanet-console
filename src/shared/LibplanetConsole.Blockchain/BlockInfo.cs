using LibplanetConsole.Grpc.Blockchain;
using static LibplanetConsole.Grpc.TypeUtility;

#if LIBPLANET_NODE
namespace LibplanetConsole.Node;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client;
#elif LIBPLANET_CONSOLE
namespace LibplanetConsole.Console;
#else
#error LIBPLANET_NODE, LIBPLANET_CLIENT, or LIBPLANET_CONSOLE must be defined.
#endif

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
