using Libplanet.Crypto;
using Libplanet.Types.Blocks;

namespace LibplanetConsole.Nodes.Serializations;

public sealed record class NodeInfo
{
    public string AppProtocolVersion { get; init; } = string.Empty;

    public string SwarmEndPoint { get; init; } = string.Empty;

    public string ConsensusEndPoint { get; init; } = string.Empty;

    public Address Address { get; init; }

    public BlockHash GenesisHash { get; init; }

    public BlockHash TipHash { get; init; }

    public bool IsRunning { get; init; }

    public int ProcessId { get; init; } = Environment.ProcessId;

    public BoundPeerInfo[] Peers { get; init; } = [];
}
