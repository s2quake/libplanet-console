using LibplanetConsole.BlockChain;
using LibplanetConsole.Node.Grpc;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Node;

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

    public static implicit operator NodeInfo(NodeInfoProto nodeInfo) => new()
    {
        ProcessId = nodeInfo.ProcessId,
        AppProtocolVersion = nodeInfo.AppProtocolVersion,
        Address = ToAddress(nodeInfo.Address),
        GenesisHash = ToBlockHash(nodeInfo.GenesisHash),
        Tip = nodeInfo.Tip,
        IsRunning = nodeInfo.IsRunning,
    };

    public static implicit operator NodeInfoProto(NodeInfo nodeInfo) => new()
    {
        ProcessId = nodeInfo.ProcessId,
        AppProtocolVersion = nodeInfo.AppProtocolVersion,
        Address = ToGrpc(nodeInfo.Address),
        GenesisHash = ToGrpc(nodeInfo.GenesisHash),
        Tip = nodeInfo.Tip,
        IsRunning = nodeInfo.IsRunning,
    };
}
