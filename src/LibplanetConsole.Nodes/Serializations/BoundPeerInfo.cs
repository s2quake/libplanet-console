using Libplanet.Net;

namespace LibplanetConsole.Nodes.Serializations;

public record class BoundPeerInfo
{
    public BoundPeerInfo(BoundPeer peer)
    {
        EndPoint = $"{peer.EndPoint.Host}:{peer.EndPoint.Port}";
    }

    public BoundPeerInfo()
    {
    }

    public string EndPoint { get; init; } = string.Empty;
}
