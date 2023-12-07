using Libplanet.Net;

namespace OnBoarding.ConsoleHost.Serializations;

record class BoundPeerInfo
{
    public BoundPeerInfo(BoundPeer peer)
    {
        EndPoint = $"{peer.EndPoint.Host}:{peer.EndPoint.Port}";
    }

    public string EndPoint { get; }
}
