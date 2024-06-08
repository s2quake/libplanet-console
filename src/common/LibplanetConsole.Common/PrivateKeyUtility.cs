using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
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

    public static PrivateKey ParseWithFallback(string text)
        => text == string.Empty ? new PrivateKey() : Parse(text);

    public static bool TryParse(string text, [MaybeNullWhen(false)] out PrivateKey privateKey)
    {
        try
        {
            privateKey = Parse(text);
            return true;
        }
        catch
        {
            privateKey = null;
            return false;
        }
    }

    public static string ToString(PrivateKey privateKey) =>
        ByteUtil.Hex(privateKey.ByteArray);

    public static PrivateKey FromSecureString(SecureString secureString)
    {
        var ptr = IntPtr.Zero;
        try
        {
            ptr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
            var text = Marshal.PtrToStringUni(ptr)!;
            var bytes = ByteUtil.ParseHex(text);
            return new(bytes);
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(ptr);
        }
    }

    public static SecureString ToSecureString(PrivateKey privateKey)
    {
        var secureString = new SecureString();
        var text = ByteUtil.Hex(privateKey.ByteArray);
        foreach (var item in text)
        {
            secureString.AppendChar(item);
        }

        return secureString;
    }

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

    public static byte[] Sign(PrivateKey privateKey, object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        var bytes = Encoding.UTF8.GetBytes(json);
        return privateKey.Sign(bytes);
    }
}
