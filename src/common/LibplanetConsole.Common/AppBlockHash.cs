using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Libplanet.Types.Blocks;
using LibplanetConsole.Common.Converters;

namespace LibplanetConsole.Common;

[JsonConverter(typeof(AppBlockHashJsonConverter))]
public readonly record struct AppBlockHash : IFormattable
{
    private readonly BlockHash _blockHash;

    public AppBlockHash(BlockHash blockHash) => _blockHash = blockHash;

    public static explicit operator AppBlockHash(BlockHash blockHash)
        => new(blockHash);

    public static explicit operator BlockHash(AppBlockHash blockHash)
        => blockHash._blockHash;

    public static string ToString(AppBlockHash? blockHash)
        => blockHash?.ToString() ?? string.Empty;

    public static AppBlockHash Parse(string text) => new(BlockHash.FromString(text));

    public static AppBlockHash? ParseOrDefault(string text)
        => text == string.Empty ? null : Parse(text);

    public static bool TryParse(string text, [MaybeNullWhen(false)] out AppBlockHash blockHash)
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

    public override string ToString() => _blockHash.ToString();

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (format is "S")
        {
            return _blockHash.ToString()[..8];
        }

        return ToString();
    }
}
