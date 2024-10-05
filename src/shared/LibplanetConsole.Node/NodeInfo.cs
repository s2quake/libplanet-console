using GrpcNodeInfo = LibplanetConsole.Node.Grpc.NodeInfo;

namespace LibplanetConsole.Node;

public readonly record struct NodeInfo
{
    public required int ProcessId { get; init; }

    public string AppProtocolVersion { get; init; }

    public string SwarmEndPoint { get; init; }

    public string ConsensusEndPoint { get; init; }

    public Address Address { get; init; }

    public BlockHash GenesisHash { get; init; }

    public BlockHash TipHash { get; init; }

    public bool IsRunning { get; init; }

    public static NodeInfo Empty { get; } = new NodeInfo
    {
        ProcessId = -1,
        AppProtocolVersion = string.Empty,
        SwarmEndPoint = string.Empty,
        ConsensusEndPoint = string.Empty,
    };

    public static implicit operator GrpcNodeInfo(NodeInfo nodeInfo)
    {
        return new GrpcNodeInfo
        {
            ProcessId = nodeInfo.ProcessId,
            AppProtocolVersion = nodeInfo.AppProtocolVersion,
            SwarmEndPoint = nodeInfo.SwarmEndPoint,
            ConsensusEndPoint = nodeInfo.ConsensusEndPoint,
            Address = nodeInfo.Address.ToHex(),
            GenesisHash = nodeInfo.GenesisHash.ToString(),
            TipHash = nodeInfo.TipHash.ToString(),
            IsRunning = nodeInfo.IsRunning,
        };
    }

    public static implicit operator NodeInfo(GrpcNodeInfo nodeInfo)
    {
        return new NodeInfo
        {
            ProcessId = nodeInfo.ProcessId,
            AppProtocolVersion = nodeInfo.AppProtocolVersion,
            SwarmEndPoint = nodeInfo.SwarmEndPoint,
            ConsensusEndPoint = nodeInfo.ConsensusEndPoint,
            Address = new Address(nodeInfo.Address),
            GenesisHash = BlockHash.FromString(nodeInfo.GenesisHash),
            TipHash = BlockHash.FromString(nodeInfo.TipHash),
            IsRunning = nodeInfo.IsRunning,
        };
    }
}
