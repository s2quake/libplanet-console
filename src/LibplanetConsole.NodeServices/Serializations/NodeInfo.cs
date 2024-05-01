using LibplanetConsole.Common;

namespace LibplanetConsole.NodeServices.Serializations;

public record class NodeInfo
{
    public NodeInfo(NodeBase nodeBase)
    {
        PrivateKey = PrivateKeyUtility.ToString(nodeBase.PrivateKey);
        PublicKey = PublicKeyUtility.ToString(nodeBase.PublicKey);
        Address = $"{nodeBase.Address}";
        if (nodeBase.IsRunning == true)
        {
            AppProtocolVersion = $"{nodeBase.AppProtocolVersion}";
            SwarmEndPoint = DnsEndPointUtility.ToString(nodeBase.SwarmEndPoint);
            ConsensusEndPoint = DnsEndPointUtility.ToString(nodeBase.ConsensusEndPoint);
            GenesisHash = $"{nodeBase.BlockChain.Genesis.Hash}";
            TipHash = $"{nodeBase.BlockChain.Tip.Hash}";
            Peers = [.. nodeBase.Peers.Select(peer => new BoundPeerInfo(peer))];
        }
    }

    public NodeInfo()
    {
    }

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
