using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Libplanet.Common;
using Libplanet.Types.Tx;
using LibplanetConsole.Common.Converters;

namespace LibplanetConsole.Common;

[JsonConverter(typeof(AppIdJsonConverter))]
public readonly record struct AppId : IFormattable
{
    private readonly ImmutableArray<byte> _bytes;

    public AppId(byte[] bytes) => _bytes = [.. bytes];

    public AppId(ImmutableArray<byte> bytes) => _bytes = bytes;

    public AppId(TxId txId) => _bytes = txId.ByteArray;

    public static explicit operator AppId(TxId txId) => new(txId);

    public static explicit operator TxId(AppId id) => new(id._bytes);

    public static string ToString(AppId? blockHash)
        => blockHash?.ToString() ?? string.Empty;

    public static AppId Parse(string text) => new(ByteUtil.ParseHexToImmutable(text));

    public static AppId? ParseOrDefault(string text)
        => text == string.Empty ? null : Parse(text);

    public static bool TryParse(string text, [MaybeNullWhen(false)] out AppId blockHash)
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
