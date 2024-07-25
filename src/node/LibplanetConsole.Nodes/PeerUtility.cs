using System.Net;
using Libplanet.Crypto;
using Libplanet.Net;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes;

internal static class PeerUtility
{
    public static BoundPeer ToBoundPeer(AppPeer peer)
        => new((PublicKey)peer.PublicKey, (DnsEndPoint)peer.EndPoint);

    public static AppPeer ToAppPeer(BoundPeer boundPeer)
        => new((AppPublicKey)boundPeer.PublicKey, (AppEndPoint)boundPeer.EndPoint);
}
