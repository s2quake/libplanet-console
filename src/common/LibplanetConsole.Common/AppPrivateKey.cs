using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bencodex;
using Bencodex.Types;
using Libplanet.Common;
using Libplanet.Crypto;
using LibplanetConsole.Common.Converters;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Common;

[JsonConverter(typeof(AppPrivateKeyJsonConverter))]
public sealed record class AppPrivateKey
{
    private static readonly Codec _codec = new();
    private readonly PrivateKey _privateKey;

    public AppPrivateKey(PrivateKey privateKey) => _privateKey = privateKey;

    public AppPrivateKey(byte[] bytes)
        : this(new PrivateKey(bytes))
    {
    }

    public AppPrivateKey()
        : this(new PrivateKey())
    {
    }

    public AppPublicKey PublicKey => (AppPublicKey)_privateKey.PublicKey;

    public AppAddress Address => (AppAddress)_privateKey.PublicKey.Address;

    public static explicit operator AppPrivateKey(PrivateKey privateKey)
        => new(privateKey);

    public static explicit operator PrivateKey(AppPrivateKey privateKey)
        => privateKey._privateKey;

    public static string ToString(AppPrivateKey? privateKey)
        => privateKey is not null ? ByteUtil.Hex(privateKey.ToByteArray()) : string.Empty;

    public static AppPrivateKey Parse(string text) => new(new PrivateKey(text));

    public static AppPrivateKey? ParseOrDefault(string text)
        => text == string.Empty ? null : Parse(text);

    public static AppPrivateKey ParseOrRandom(string text)
        => text == string.Empty ? new AppPrivateKey() : Parse(text);

    public static bool TryParse(string text, [MaybeNullWhen(false)] out AppPrivateKey privateKey)
    {
        try
        {
            privateKey = Parse(text);
            return true;
        }
        catch
        {
            privateKey = default;
            return false;
        }
    }

    public override string? ToString()
    {
#if DEBUG
        System.Diagnostics.Trace.TraceWarning(
            "AppPrivateKey.ToString() is called. It may be a security risk.");
#endif
        return base.ToString();
    }

    public byte[] ToByteArray() => [.. _privateKey.ByteArray];

    public static AppPrivateKey FromSecureString(SecureString secureString)
    {
        using var ptr = new StringPointer(secureString);
        var text = ptr.GetString();
        var bytes = ByteUtil.ParseHex(text);
        return new(bytes);
    }

    public SecureString ToSecureString()
    {
        var secureString = new SecureString();
        var text = ByteUtil.Hex(_privateKey.ByteArray);
        secureString.AppendString(text);
        return secureString;
    }

    public object? Decrypt(string text, Type type)
    {
        var bytes = ByteUtil.ParseHex(text);
        var decrypted = _privateKey.Decrypt(bytes);
        var json = Encoding.UTF8.GetString(decrypted);
        return JsonSerializer.Deserialize(json, type);
    }

    public T Decrypt<T>(string text)
        where T : notnull => Decrypt(text, typeof(T)) switch
        {
            T result => result,
            _ => throw new InvalidOperationException($"Failed to decrypt {text} as {typeof(T)}."),
        };

    public byte[] Sign(object obj)
    {
        if (obj is IValue value)
        {
            return _privateKey.Sign(_codec.Encode(value));
        }

        var json = JsonSerializer.Serialize(obj);
        var bytes = Encoding.UTF8.GetBytes(json);
        return _privateKey.Sign(bytes);
    }
}
