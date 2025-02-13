using System.Collections.Immutable;

namespace LibplanetConsole.Node.Explorer.Types;

internal sealed class HexValue(byte[] bytes)
{
    private readonly byte[] _bytes = bytes;

    public static implicit operator HexValue(byte[] bytes) => new(bytes);

    public static implicit operator byte[](HexValue hexValue) => hexValue._bytes;

    public static HexValue Parse(string hex) => ByteUtil.ParseHex(hex);

    public override string ToString() => ByteUtil.Hex(_bytes);

    public ImmutableArray<byte> ToImmutableArray() => [.. _bytes];
}
