using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Libplanet.Crypto;
using LibplanetConsole.Common.Converters;

namespace LibplanetConsole.Common;

[JsonConverter(typeof(AppAddressJsonConverter))]
[TypeConverter(typeof(AppAddressConverter))]
public readonly record struct AppAddress : IFormattable
{
    private readonly Address _address;

    public AppAddress(Address address) => _address = address;

    public AppAddress(byte[] bytes)
        : this(new Address(bytes))
    {
    }

    public static explicit operator AppAddress(Address address)
        => new(address);

    public static explicit operator Address(AppAddress address)
        => address._address;

    public static string ToString(AppAddress? address)
        => address?.ToString() ?? string.Empty;

    public static AppAddress Parse(string text) => new(new Address(text));

    public static AppAddress? ParseOrDefault(string text)
        => text == string.Empty ? null : Parse(text);

    public static bool TryParse(string text, [MaybeNullWhen(false)] out AppAddress address)
    {
        try
        {
            address = Parse(text);
            return true;
        }
        catch
        {
            address = default;
            return false;
        }
    }

    public override string ToString() => _address.ToString();

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (format is "S")
        {
            return $"{_address}"[0..8];
        }

        return ToString();
    }

    public string ToShortString() => ToString(format: "S", formatProvider: null);

    public AppAddress Derive(byte[] key)
    {
        var bytes = _address.ToByteArray();
        var hashed = GetHahsed(key, bytes);
        return new AppAddress(hashed);

        static byte[] GetHahsed(byte[] key, byte[] bytes)
        {
            using var hmac = new HMACSHA1(key);
            return hmac.ComputeHash(bytes);
        }
    }

    public AppAddress Derive(string key) => Derive(Encoding.UTF8.GetBytes(key));
}
