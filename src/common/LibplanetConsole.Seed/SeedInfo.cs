using System.Text.Json.Serialization;
using LibplanetConsole.Seed.Converters;

namespace LibplanetConsole.Seed;

public readonly record struct SeedInfo
{
    public static SeedInfo Empty { get; } = default;

    [JsonConverter(typeof(BoundPeerJsonConverter))]
    public BoundPeer BlocksyncSeedPeer { get; init; }

    [JsonConverter(typeof(BoundPeerJsonConverter))]
    public BoundPeer ConsensusSeedPeer { get; init; }
}
