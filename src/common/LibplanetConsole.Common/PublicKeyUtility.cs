using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using Libplanet.Common;
using Libplanet.Crypto;

namespace LibplanetConsole.Common;

public static class PublicKeyUtility
{
    public static PublicKey Parse(string text) => PublicKey.FromHex(text);

    public static bool TryParse(string text, [MaybeNullWhen(false)] out PublicKey publicKey)
    {
        try
        {
            publicKey = Parse(text);
            return true;
        }
        catch
        {
            publicKey = null;
            return false;
        }
    }

    public static string ToString(PublicKey publicKey)
        => publicKey.ToHex(compress: false);

    public static string Encrypt(PublicKey publicKey, object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        var encodings = Encoding.UTF8.GetBytes(json);
        var encrypted = publicKey.Encrypt(encodings);
        return ByteUtil.Hex(encrypted);
    }

    public static bool Verify(PublicKey publicKey, object obj, byte[] signature)
    {
        var json = JsonSerializer.Serialize(obj);
        var bytes = Encoding.UTF8.GetBytes(json);
        return publicKey.Verify(bytes, signature);
    }
}
