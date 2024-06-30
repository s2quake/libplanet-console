using System.Net;
using System.Text.Json.Serialization;
using Libplanet.Crypto;
using Libplanet.Net;
using LibplanetConsole.Common.Converters;

namespace LibplanetConsole.Common;

[JsonConverter(typeof(AppPeerJsonConverter))]
public readonly struct AppPeer(AppPublicKey publicKey, AppEndPoint endPoint)
{
    public AppPublicKey PublicKey { get; } = publicKey;

    public AppEndPoint EndPoint { get; } = endPoint;

    public AppAddress Address => PublicKey.Address;

    public static explicit operator BoundPeer(AppPeer peer)
        => new((PublicKey)peer.PublicKey, (DnsEndPoint)peer.EndPoint);

    public static explicit operator AppPeer(BoundPeer boundPeer)
        => new((AppPublicKey)boundPeer.PublicKey, (AppEndPoint)boundPeer.EndPoint);

    public static string ToString(AppPeer? peer)
        => peer?.ToString() ?? string.Empty;

    public static AppPeer Parse(string text)
    {
        var items = text.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (items.Length == 2)
        {
            var publicKey = AppPublicKey.Parse(items[0]);
            var endPoint = AppEndPoint.Parse(items[1].Trim());
            return new AppPeer(publicKey, endPoint);
        }

        throw new FormatException($"'{text}' is not supported.");
    }

    public static AppPeer? ParseOrDefault(string text) => text == string.Empty ? null : Parse(text);

    public override string ToString() => $"{PublicKey}, {EndPoint}";
}
