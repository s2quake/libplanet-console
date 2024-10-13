using LibplanetConsole.Blockchain;
using LibplanetConsole.Node.Grpc;

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

    public static implicit operator NodeInfo(NodeInformation nodeInfo)
    {
        return new NodeInfo
        {
            ProcessId = nodeInfo.ProcessId,
            AppProtocolVersion = nodeInfo.AppProtocolVersion,
            BlocksyncPort = nodeInfo.BlocksyncPort,
            ConsensusPort = nodeInfo.ConsensusPort,
            Address = new Address(nodeInfo.Address),
            GenesisHash = BlockHash.FromString(nodeInfo.GenesisHash),
            Tip = new BlockInfo
            {
                Height = nodeInfo.TipHeight,
                Hash = BlockHash.FromString(nodeInfo.TipHash),
                Miner = new Address(nodeInfo.TipMiner),
            },
            IsRunning = nodeInfo.IsRunning,
        };
    }

    public static implicit operator NodeInformation(NodeInfo nodeInfo)
    {
        return new NodeInformation
        {
            ProcessId = nodeInfo.ProcessId,
            AppProtocolVersion = nodeInfo.AppProtocolVersion,
            BlocksyncPort = nodeInfo.BlocksyncPort,
            ConsensusPort = nodeInfo.ConsensusPort,
            Address = nodeInfo.Address.ToHex(),
            GenesisHash = nodeInfo.GenesisHash.ToString(),
            TipHash = nodeInfo.Tip.Hash.ToString(),
            TipHeight = nodeInfo.Tip.Height,
            TipMiner = nodeInfo.Tip.Miner.ToHex(),
            IsRunning = nodeInfo.IsRunning,
        };
    }
}
