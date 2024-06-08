using System.Text;
using Bencodex;
using Bencodex.Types;
using Libplanet.Common;
using Libplanet.Crypto;
using LibplanetConsole.Common.Serializations;

namespace LibplanetConsole.Common;

public sealed record class GenesisOptions
{
    private static readonly Codec _codec = new();

    public PrivateKey GenesisKey { get; init; } = new PrivateKey();

    public PublicKey[] GenesisValidators { get; init; } = [];

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.MinValue;

    public override string ToString()
    {
        var s = JsonUtility.SerializeObject((GenesisInfo)this);
        var bytes = Encoding.UTF8.GetBytes(s);
        return ByteUtil.Hex(bytes);
    }

    public string ToJson() => JsonUtility.SerializeObject((GenesisInfo)this);

    public static GenesisOptions Parse(string text)
    {
        var bytes = ByteUtil.ParseHex(text);
        var s = Encoding.UTF8.GetString(bytes);
        return JsonUtility.DeserializeObject<GenesisInfo>(s);
    }

    public byte[] ToByteArray()
    {
        var genesisKey = GenesisKey.ToByteArray();
        var genesisValidators
            = new List(GenesisValidators.Select(item => item.ToImmutableArray(false)));
        var timestamp = Timestamp.ToUnixTimeMilliseconds();
        var dictionary = Dictionary.Empty
                            .Add(nameof(GenesisKey), genesisKey)
                            .Add(nameof(GenesisValidators), genesisValidators)
                            .Add(nameof(Timestamp), timestamp);
        return _codec.Encode(dictionary);
    }
}
