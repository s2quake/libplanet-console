using Libplanet.Net;

namespace LibplanetConsole.Nodes.Serializations;

public readonly record struct BoundPeerInfo
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
