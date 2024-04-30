using System.ComponentModel.Composition;
using JSSoft.Communication;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Serializations;

namespace LibplanetConsole.NodeHost;

[Export(typeof(IService))]
[method: ImportingConstructor]
internal sealed class SeedService(IApplication application, INode node)
    : ServerService<ISeedService>, ISeedService
{
    public Task<string> GetNodeEndPointAsync(CancellationToken cancellationToken)
    {
        return application.InvokeAsync(() => application.Info.EndPoint);
    }

    public Task<SeedInfo> GetSeedAsync(CancellationToken cancellationToken)
    {
        var nodeInfo = node.Info;
        var publicKey = node.PrivateKey.PublicKey;
        var seedPeer = BoundPeerUtility.Create(publicKey, nodeInfo.SwarmEndPoint);
        var consensusSeedPeer = BoundPeerUtility.Create(publicKey, nodeInfo.ConsensusEndPoint);

        return application.InvokeAsync(() => new SeedInfo
        {
            GenesisOptions = node.NodeOptions.GenesisOptions,
            SeedPeer = BoundPeerUtility.ToString(seedPeer),
            ConsensusSeedPeer = BoundPeerUtility.ToString(consensusSeedPeer),
        });
    }
}
