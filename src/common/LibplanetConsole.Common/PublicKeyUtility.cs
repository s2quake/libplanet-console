using Libplanet.Crypto;

namespace LibplanetConsole.Common;

public static class PublicKeyUtility
{
    public static PublicKey Parse(string text) => PublicKey.FromHex(text);

    public static string ToString(PublicKey publicKey)
        => publicKey.ToHex(compress: false);
}
