using Libplanet.Net;

namespace LibplanetConsole.Executable.Serializations;

record class BoundPeerInfo
{
    public BoundPeerInfo(BoundPeer peer)
    {
        EndPoint = $"{peer.EndPoint.Host}:{peer.EndPoint.Port}";
    }

    public string EndPoint { get; }
}
