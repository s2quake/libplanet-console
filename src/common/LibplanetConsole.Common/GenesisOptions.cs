using Bencodex;
using Bencodex.Types;

namespace LibplanetConsole.Common;

public sealed record class GenesisOptions
{
    public static readonly AppPrivateKey AppProtocolKey
        = AppPrivateKey.Parse("2a15e7deaac09ce631e1faa184efadb175b6b90989cf1faed9dfc321ad1db5ac");

    public const int AppProtocolVersion = 1;

    private static readonly Codec _codec = new();

    public AppPrivateKey GenesisKey { get; init; } = new();

    public AppPublicKey[] GenesisValidators { get; init; } = [];

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.MinValue;

    public byte[] ToByteArray()
    {
        var genesisKey = GenesisKey.ToByteArray();
        var genesisValidators
            = new List(GenesisValidators.Select(item => item.ToByteArray()));
        var timestamp = Timestamp.ToUnixTimeMilliseconds();
        var dictionary = Dictionary.Empty
                            .Add(nameof(GenesisKey), genesisKey)
                            .Add(nameof(GenesisValidators), genesisValidators)
                            .Add(nameof(Timestamp), timestamp);
        return _codec.Encode(dictionary);
    }
}
