namespace LibplanetConsole.Nodes.Serializations;

public record class NodeInfo
{
    public string AppProtocolVersion { get; init; } = string.Empty;

    public string SwarmEndPoint { get; init; } = string.Empty;

    public string ConsensusEndPoint { get; init; } = string.Empty;

    public string PrivateKey { get; init; } = string.Empty;

    public string PublicKey { get; init; } = string.Empty;

    public string Address { get; init; } = string.Empty;

    public string GenesisHash { get; init; } = string.Empty;

    public string TipHash { get; init; } = string.Empty;

    public int ProcessId { get; init; } = Environment.ProcessId;

    public BoundPeerInfo[] Peers { get; init; } = [];
}
