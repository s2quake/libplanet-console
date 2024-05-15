using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Libplanet.Common;
using Libplanet.Crypto;

namespace LibplanetConsole.Common;

public static class PrivateKeyUtility
{
    public static PrivateKey Create(string name)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(name));
        return new(bytes);
    }

    public static PrivateKey Parse(string text) => new(text);

    public static string ToString(PrivateKey privateKey) =>
        ByteUtil.Hex(privateKey.ByteArray);

    public static object? Decrypt(PrivateKey privateKey, string text, Type type)
    {
        var bytes = ByteUtil.ParseHex(text);
        var decrypted = privateKey.Decrypt(bytes);
        var json = Encoding.UTF8.GetString(decrypted);
        return JsonSerializer.Deserialize(json, type);
    }

    public static T Decrypt<T>(PrivateKey privateKey, string text)
        where T : notnull
    {
        if (Decrypt(privateKey, text, typeof(T)) is T result)
        {
            return result;
        }

        throw new InvalidOperationException($"Failed to decrypt {text} as {typeof(T)}.");
    }
}
