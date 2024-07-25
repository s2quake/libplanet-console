using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Libplanet.Common;
using Libplanet.Types.Blocks;
using LibplanetConsole.Common.Converters;

namespace LibplanetConsole.Common;

[JsonConverter(typeof(AppHashJsonConverter))]
public readonly record struct AppHash : IFormattable
{
    private readonly ImmutableArray<byte> _bytes;

    public AppHash(byte[] bytes)
    {
        _bytes = [.. bytes];
    }

    public AppHash(ImmutableArray<byte> bytes)
    {
        _bytes = bytes;
    }

    public AppHash(BlockHash blockHash) => _bytes = blockHash.ByteArray;

    public static explicit operator AppHash(BlockHash blockHash) => new(blockHash);

    public static explicit operator AppHash(HashDigest<SHA256> hashDigest)
        => new(hashDigest.ByteArray);

    public static explicit operator BlockHash(AppHash hash) => new(hash._bytes);

    public static explicit operator HashDigest<SHA256>(AppHash hash) => new(hash._bytes);

    public static string ToString(AppHash? blockHash)
        => blockHash?.ToString() ?? string.Empty;

    public static AppHash Parse(string text) => new(ByteUtil.ParseHexToImmutable(text));

    public static AppHash? ParseOrDefault(string text)
        => text == string.Empty ? null : Parse(text);

    public static bool TryParse(string text, [MaybeNullWhen(false)] out AppHash blockHash)
    {
        try
        {
            blockHash = Parse(text);
            return true;
        }
        catch
        {
            blockHash = default;
            return false;
        }
    }

    public override string ToString() => ByteUtil.Hex(_bytes);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (format is "S")
        {
            return ToString()[..8];
        }

        return ToString();
    }
}
