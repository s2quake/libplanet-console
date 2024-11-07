#if LIBPLANET_NODE
namespace LibplanetConsole.Node;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client;
#elif LIBPLANET_CONSOLE

namespace LibplanetConsole.Console;
#else
#error LIBPLANET_NODE, LIBPLANET_CLIENT, or LIBPLANET_CONSOLE must be defined.
#endif

public static partial class BlockUtility
{
    private static readonly Codec _codec = new();

    public static Block LoadGenesisBlock(string genesisPath) => genesisPath switch
    {
        { } path when Uri.TryCreate(path, UriKind.Absolute, out var uri)
            => LoadGenesisBlockFromUrl(uri),
        { } path => LoadGenesisBlockFromFile(path),
        _ => throw new NotSupportedException(),
    };

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

    public static string ToString(Block block)
    {
        var @byte = BlockMarshaler.MarshalBlock(block);
        return ByteUtil.Hex(_codec.Encode(@byte));
    }

    private static Block LoadGenesisBlockFromFile(string genesisPath)
    {
        var rawBlock = File.ReadAllBytes(Path.GetFullPath(genesisPath));
        var blockDict = (Dictionary)_codec.Decode(rawBlock);
        return BlockMarshaler.UnmarshalBlock(blockDict);
    }

    private static Block LoadGenesisBlockFromUrl(Uri genesisBlockUri)
    {
        using var client = new HttpClient();
        var rawBlock = client.GetByteArrayAsync(genesisBlockUri).Result;
        var blockDict = (Dictionary)_codec.Decode(rawBlock);
        return BlockMarshaler.UnmarshalBlock(blockDict);
    }
}
