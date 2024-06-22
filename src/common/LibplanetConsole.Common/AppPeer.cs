using System.Net;
using Libplanet.Crypto;
using Libplanet.Net;

namespace LibplanetConsole.Common;

public readonly struct AppPeer(PublicKey publicKey, AppEndPoint endPoint)
{
    public PublicKey PublicKey { get; } = publicKey;

    public AppEndPoint EndPoint { get; } = endPoint;

    public Address Address => PublicKey.Address;

    public static explicit operator BoundPeer(AppPeer peer)
        => new(peer.PublicKey, (DnsEndPoint)peer.EndPoint);

    public static explicit operator AppPeer(BoundPeer boundPeer)
        => new(boundPeer.PublicKey, (AppEndPoint)boundPeer.EndPoint);

    public static string ToString(AppPeer? peer)
        => peer?.ToString() ?? string.Empty;

    public static AppPeer Parse(string text)
    {
        var items = text.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (items.Length == 2)
        {
            var publicKey = PublicKeyUtility.Parse(items[0]);
            var endPoint = AppEndPoint.Parse(items[1].Trim());
            return new AppPeer(publicKey, endPoint);
        }

        throw new FormatException($"'{text}' is not supported.");
    }

    public static AppPeer? ParseOrDefault(string text) => text == string.Empty ? null : Parse(text);

    public override string ToString() => $"{PublicKeyUtility.ToString(PublicKey)}, {EndPoint}";
}
