using System.Net;
using Libplanet.Crypto;
using Libplanet.Net;
using LibplanetConsole.Common;

namespace LibplanetConsole.Seed;

public static class BoundPeerUtility
{
    public static string ToString(BoundPeer? boundPeer)
        => boundPeer is not null
            ? $"{boundPeer.PublicKey.ToHex(compress: false)}, {boundPeer.EndPoint.Host}:{boundPeer.EndPoint.Port}"
            : string.Empty;

    public static BoundPeer Parse(string text)
    {
        var items = text.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (items.Length == 2)
        {
            var publicKey = PublicKey.FromHex(items[0]);
            var endPoint = (DnsEndPoint)EndPointUtility.Parse(items[1].Trim());
            return new BoundPeer(publicKey, endPoint);
        }

        throw new FormatException($"'{text}' is not supported.");
    }

    public static BoundPeer? ParseOrDefault(string text) => text == string.Empty ? null : Parse(text);
}
