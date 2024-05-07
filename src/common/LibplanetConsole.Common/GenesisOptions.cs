using System.Text;
using Libplanet.Common;
using Libplanet.Crypto;
using LibplanetConsole.Common.Serializations;

namespace LibplanetConsole.Common;

public record class GenesisOptions
{
    public PrivateKey GenesisKey { get; init; } = new PrivateKey();

    public PublicKey[] GenesisValidators { get; init; } = [];

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.MinValue;

    public override string ToString()
    {
        var s = JsonUtility.SerializeObject((GenesisOptionsInfo)this);
        var bytes = Encoding.UTF8.GetBytes(s);
        return ByteUtil.Hex(bytes);
    }

    public static GenesisOptions Parse(string text)
    {
        var bytes = ByteUtil.ParseHex(text);
        var s = Encoding.UTF8.GetString(bytes);
        return JsonUtility.DeserializeObject<GenesisOptionsInfo>(s);
    }
}
