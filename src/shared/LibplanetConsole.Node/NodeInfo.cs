using LibplanetConsole.Blockchain;
using LibplanetConsole.Node.Grpc;
using static LibplanetConsole.Blockchain.Grpc.TypeUtility;

namespace LibplanetConsole.Node;

public readonly record struct NodeInfo
{
    public required int ProcessId { get; init; }

    public string AppProtocolVersion { get; init; }

    public int BlocksyncPort { get; init; }

    public int ConsensusPort { get; init; }

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
            BlocksyncPort = nodeInfo.BlocksyncPort,
            ConsensusPort = nodeInfo.ConsensusPort,
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
            BlocksyncPort = nodeInfo.BlocksyncPort,
            ConsensusPort = nodeInfo.ConsensusPort,
            Address = ToGrpc(nodeInfo.Address),
            GenesisHash = ToGrpc(nodeInfo.GenesisHash),
            Tip = nodeInfo.Tip,
            IsRunning = nodeInfo.IsRunning,
        };
    }
}
