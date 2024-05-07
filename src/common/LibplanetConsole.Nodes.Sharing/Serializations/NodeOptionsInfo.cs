using LibplanetConsole.Common;
using LibplanetConsole.Common.Serializations;

namespace LibplanetConsole.Nodes.Serializations;

public record struct NodeOptionsInfo
{
    public GenesisOptionsInfo GenesisOptions { get; set; }

    public string BlocksyncSeedPeer { get; set; }

    public string ConsensusSeedPeer { get; set; }

    public static implicit operator NodeOptions(NodeOptionsInfo info)
    {
        return new NodeOptions
        {
            GenesisOptions = info.GenesisOptions,
            BlocksyncSeedPeer = BoundPeerUtility.GetSafeBoundPeer(info.BlocksyncSeedPeer),
            ConsensusSeedPeer = BoundPeerUtility.GetSafeBoundPeer(info.ConsensusSeedPeer),
        };
    }

    public static implicit operator NodeOptionsInfo(NodeOptions nodeOptions)
    {
        return new NodeOptionsInfo
        {
            GenesisOptions = nodeOptions.GenesisOptions,
            BlocksyncSeedPeer = BoundPeerUtility.ToSafeString(nodeOptions.BlocksyncSeedPeer),
            ConsensusSeedPeer = BoundPeerUtility.ToSafeString(nodeOptions.ConsensusSeedPeer),
        };
    }

    public static implicit operator NodeOptionsInfo(SeedInfo seedInfo)
    {
        return new NodeOptionsInfo
        {
            GenesisOptions = seedInfo.GenesisOptions,
            BlocksyncSeedPeer = seedInfo.BlocksyncSeedPeer,
            ConsensusSeedPeer = seedInfo.ConsensusSeedPeer,
        };
    }
}
