using LibplanetConsole.Common;
using LibplanetConsole.Common.Serializations;

namespace LibplanetConsole.NodeServices.Serializations;

public record struct NodeOptionsInfo
{
    public GenesisOptionsInfo GenesisOptions { get; set; }

    public string SeedPeer { get; set; }

    public string ConsensusSeedPeer { get; set; }

    public static implicit operator NodeOptions(NodeOptionsInfo info)
    {
        return new NodeOptions
        {
            GenesisOptions = info.GenesisOptions,
            SeedPeer = BoundPeerUtility.GetSafeBoundPeer(info.SeedPeer),
            ConsensusSeedPeer = BoundPeerUtility.GetSafeBoundPeer(info.ConsensusSeedPeer),
        };
    }

    public static implicit operator NodeOptionsInfo(NodeOptions nodeOptions)
    {
        return new NodeOptionsInfo
        {
            GenesisOptions = nodeOptions.GenesisOptions,
            SeedPeer = BoundPeerUtility.ToSafeString(nodeOptions.SeedPeer),
            ConsensusSeedPeer = BoundPeerUtility.ToSafeString(nodeOptions.ConsensusSeedPeer),
        };
    }

    public static implicit operator NodeOptionsInfo(SeedInfo seedInfo)
    {
        return new NodeOptionsInfo
        {
            GenesisOptions = seedInfo.GenesisOptions,
            SeedPeer = seedInfo.SeedPeer,
            ConsensusSeedPeer = seedInfo.ConsensusSeedPeer,
        };
    }
}
