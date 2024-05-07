using Libplanet.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Nodes.Serializations;

namespace LibplanetConsole.Nodes;

public record class NodeOptions
{
    public static NodeOptions Default { get; } = new NodeOptions();

    public GenesisOptions GenesisOptions { get; init; } = GenesisOptions.Default;

    public BoundPeer? BlocksyncSeedPeer { get; init; }

    public BoundPeer? ConsensusSeedPeer { get; init; }
}
