using LibplanetConsole.Grpc.Node;
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

public readonly record struct NodeInfo
{
    public required int ProcessId { get; init; }

    public string AppProtocolVersion { get; init; }

    public Address Address { get; init; }

    public BlockHash GenesisHash { get; init; }

    public BlockInfo Tip { get; init; }

    public bool IsRunning { get; init; }

    public static NodeInfo Empty { get; } = new NodeInfo
    {
        ProcessId = -1,
        AppProtocolVersion = string.Empty,
        Tip = BlockInfo.Empty,
    };

    public static implicit operator NodeInfo(NodeInfoProto nodeInfo)
    {
        return new NodeInfo
        {
            ProcessId = nodeInfo.ProcessId,
            AppProtocolVersion = nodeInfo.AppProtocolVersion,
            Address = ToAddress(nodeInfo.Address),
            GenesisHash = ToBlockHash(nodeInfo.GenesisHash),
            Tip = nodeInfo.Tip,
            IsRunning = nodeInfo.IsRunning,
        };
    }

    public static implicit operator NodeInfoProto(NodeInfo nodeInfo)
    {
        return new NodeInfoProto
        {
            ProcessId = nodeInfo.ProcessId,
            AppProtocolVersion = nodeInfo.AppProtocolVersion,
            Address = ToGrpc(nodeInfo.Address),
            GenesisHash = ToGrpc(nodeInfo.GenesisHash),
            Tip = nodeInfo.Tip,
            IsRunning = nodeInfo.IsRunning,
        };
    }
}
