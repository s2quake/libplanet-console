using Bencodex;
using Bencodex.Types;
using Libplanet.Types.Blocks;

namespace LibplanetConsole.Node;

public static partial class BlockUtility
{
    public static byte[] SerializeBlock(Block block)
    {
        var blockDictionary = BlockMarshaler.MarshalBlock(block);
        var codec = new Codec();
        return codec.Encode(blockDictionary);
    }

    public static Block DeserializeBlock(byte[] bytes)
    {
        var codec = new Codec();
        var value = codec.Decode(bytes);
        if (value is not Dictionary blockDictionary)
        {
            throw new InvalidCastException("The given bytes is not a block.");
        }

        return BlockMarshaler.UnmarshalBlock(blockDictionary);
    }
}
