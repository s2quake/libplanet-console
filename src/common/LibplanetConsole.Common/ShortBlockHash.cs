using Libplanet.Types.Blocks;

namespace LibplanetConsole.Common;

public readonly struct ShortBlockHash(BlockHash blockHash)
{
    private readonly string _text = blockHash.ToString()[..8];

    public static implicit operator string(ShortBlockHash shortAddress)
        => shortAddress._text;

    public static explicit operator ShortBlockHash(BlockHash address)
        => new(address);

    public override readonly string? ToString() => _text;
}
