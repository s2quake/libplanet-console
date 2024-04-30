using System.ComponentModel.Composition;
using JSSoft.Communication;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Serializations;

namespace LibplanetConsole.Executable;

[Export(typeof(IService))]
[method: ImportingConstructor]
internal sealed class SeedService(IApplication application, NodeCollection nodes)
    : ServerService<ISeedService>, ISeedService
{
    public async Task<SeedInfo> GetSeedAsync(CancellationToken cancellationToken)
    {
        var node = nodes.RandomNode();
        var nodeInfo = await node.GetInfoAsync(cancellationToken);
        var publicKey = node.PrivateKey.PublicKey;
        var seedPeer = BoundPeerUtility.Create(publicKey, nodeInfo.SwarmEndPoint);
        var consensusSeedPeer = BoundPeerUtility.Create(publicKey, nodeInfo.ConsensusEndPoint);

        return new SeedInfo
        {
            GenesisOptions = application.GenesisOptions,
            SeedPeer = BoundPeerUtility.ToString(seedPeer),
            ConsensusSeedPeer = BoundPeerUtility.ToString(consensusSeedPeer),
        };
    }

    public Task<string> GetNodeEndPointAsync(CancellationToken cancellationToken)
    {
        var node = nodes.RandomNode();
        return application.InvokeAsync(() => EndPointUtility.ToString(node.EndPoint));
    }
}
