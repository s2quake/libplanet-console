using Libplanet.Crypto;
using Libplanet.Net;

namespace LibplanetConsole.Common;

public static class BoundPeerUtility
{
    public static string ToString(BoundPeer boundPeer)
    {
        var publicKey = PublicKeyUtility.ToString(boundPeer.PublicKey);
        var endPoint = DnsEndPointUtility.ToString(boundPeer.EndPoint);
        return $"{publicKey}, {endPoint}";
    }

    public static string ToSafeString(BoundPeer? boundPeer)
         => boundPeer is not null ? ToString(boundPeer) : string.Empty;

    public static BoundPeer GetBoundPeer(string boundPeer)
    {
        var items = boundPeer.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (items.Length == 2)
        {
            var publicKey = PublicKeyUtility.Parse(items[0]);
            var endPoint = DnsEndPointUtility.GetEndPoint(items[1].Trim());
            return new BoundPeer(publicKey, endPoint);
        }

        throw new FormatException($"'{boundPeer}' is not supported.");
    }

    public static BoundPeer? GetSafeBoundPeer(string boundPeer)
    {
        try
        {
            return GetBoundPeer(boundPeer);
        }
        catch
        {
            return null;
        }
    }

    public static BoundPeer Create(PublicKey publicKey, string endPoint)
    {
        return new BoundPeer(publicKey, DnsEndPointUtility.GetEndPoint(endPoint));
    }
}
