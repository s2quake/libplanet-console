using System.Text.Json.Serialization;
using LibplanetConsole.Seed.Converters;
using LibplanetConsole.Seed.Grpc;

namespace LibplanetConsole.Seed;

public readonly record struct SeedInfo
{
    public static SeedInfo Empty { get; } = default;

    [JsonConverter(typeof(BoundPeerJsonConverter))]
    public BoundPeer BlocksyncSeedPeer { get; init; }

    [JsonConverter(typeof(BoundPeerJsonConverter))]
    public BoundPeer ConsensusSeedPeer { get; init; }

    public static implicit operator SeedInfoProto(SeedInfo seedInfo) => new()
    {
        BlocksyncSeedPeer = BoundPeerUtility.ToString(seedInfo.BlocksyncSeedPeer),
        ConsensusSeedPeer = BoundPeerUtility.ToString(seedInfo.ConsensusSeedPeer),
    };

    public static implicit operator SeedInfo(SeedInfoProto seedInfo) => new()
    {
        BlocksyncSeedPeer = BoundPeerUtility.Parse(seedInfo.BlocksyncSeedPeer),
        ConsensusSeedPeer = BoundPeerUtility.Parse(seedInfo.ConsensusSeedPeer),
    };
}
