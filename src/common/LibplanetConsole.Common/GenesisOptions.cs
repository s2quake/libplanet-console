using System.Text;
using Libplanet.Common;
using Libplanet.Crypto;
using LibplanetConsole.Common.Serializations;

namespace LibplanetConsole.Common;

public record class GenesisOptions
{
    public static readonly PrivateKey DefaultGenesisKey = PrivateKey.FromString(
        "2a15e7deaac09ce631e1faa184efadb175b6b90989cf1faed9dfc321ad1db5ac");

    public static readonly PrivateKey[] Validators =
    [
        DefaultGenesisKey,
        PrivateKey.FromString(
            "c32a94e6ae8d551f5c3cade1aa665c11065216684e965fc6955730b7e175157e"),
        PrivateKey.FromString(
            "fc00b0702c73cc9214e56b526104361edd89e0070bcf43c70b0f8e144d37782d"),
        PrivateKey.FromString(
            "8d66b5da75129709a25eb311c129ef45773e19a369b5c05ee37a152e6ec020d8"),
    ];

    public static GenesisOptions Default { get; } = new()
    {
        GenesisKey = DefaultGenesisKey,
        GenesisValidators = [.. Validators.Select(item => item.PublicKey)],
        Timestamp = DateTimeOffset.MinValue,
    };

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
