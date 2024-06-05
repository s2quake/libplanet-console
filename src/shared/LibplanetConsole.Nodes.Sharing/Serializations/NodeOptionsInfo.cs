using LibplanetConsole.Common.Serializations;

namespace LibplanetConsole.Nodes.Serializations;

public record struct NodeOptionsInfo
{
    public GenesisInfo GenesisInfo { get; set; }

    public string BlocksyncSeedPeer { get; set; }

    public string ConsensusSeedPeer { get; set; }

    public static implicit operator NodeOptionsInfo(SeedInfo seedInfo)
    {
        return new NodeOptionsInfo
        {
            GenesisInfo = seedInfo.GenesisInfo,
            BlocksyncSeedPeer = seedInfo.BlocksyncSeedPeer,
            ConsensusSeedPeer = seedInfo.ConsensusSeedPeer,
        };
    }
}
