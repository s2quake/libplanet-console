using Bencodex;
using Bencodex.Types;

namespace LibplanetConsole.Common;

public sealed record class GenesisOptions
{
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
