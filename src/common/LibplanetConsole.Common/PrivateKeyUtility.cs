using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace LibplanetConsole.Common;

public static class PrivateKeyUtility
{
    public static PrivateKey FromSecureString(SecureString secureString)
    {
        using var ptr = new StringPointer(secureString);
        var text = ptr.GetString();
        var bytes = ByteUtil.ParseHex(text);
        return new(bytes);
    }

    public static PrivateKey ParseOrRandom(string text)
        => text == string.Empty ? new PrivateKey() : new PrivateKey(text);

    public static bool TryParse(string text, [MaybeNullWhen(false)] out PrivateKey privateKey)
    {
        try
        {
            privateKey = new(text);
            return true;
        }
        catch
        {
            privateKey = default;
            return false;
        }
    }

    public static string ToString(PrivateKey? privateKey)
        => privateKey is not null ? ByteUtil.Hex(privateKey.ToByteArray()) : string.Empty;
}
