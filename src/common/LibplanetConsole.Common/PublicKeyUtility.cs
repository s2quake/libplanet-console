using System.Text;
using System.Text.Json;
using Libplanet.Common;
using Libplanet.Crypto;

namespace LibplanetConsole.Common;

public static class PublicKeyUtility
{
    public static PublicKey Parse(string text) => PublicKey.FromHex(text);

    public static string ToString(PublicKey publicKey)
        => publicKey.ToHex(compress: false);

    public static string Encrypt(PublicKey publicKey, object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        var encodings = Encoding.UTF8.GetBytes(json);
        var encrypted = publicKey.Encrypt(encodings);
        return ByteUtil.Hex(encrypted);
    }
}
