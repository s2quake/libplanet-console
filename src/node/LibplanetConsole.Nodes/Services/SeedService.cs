using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Serializations;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Nodes.Services;

[Export(typeof(ILocalService))]
[method: ImportingConstructor]
internal sealed class SeedService(IApplication application, INode node)
    : LocalService<ISeedService>, ISeedService
{
    public Task<string> GetNodeEndPointAsync(CancellationToken cancellationToken)
    {
        return application.InvokeAsync(() => application.Info.EndPoint);
    }

    public async Task<SeedInfo> GetSeedAsync(
        PublicKey publicKey, CancellationToken cancellationToken)
    {
        if (node.IsRunning == true)
        {
            var nodeInfo = node.Info;
            var blocksyncSeedPeer = node.BlocksyncSeedPeer;
            var consensusSeedPeer = node.ConsensusSeedPeer;
            var genesisOptions = (GenesisOptionsInfo)node.NodeOptions.GenesisOptions;

            return await application.InvokeAsync(() => new SeedInfo
            {
                GenesisOptions = genesisOptions.Encrypt(publicKey),
                BlocksyncSeedPeer = BoundPeerUtility.ToString(blocksyncSeedPeer),
                ConsensusSeedPeer = BoundPeerUtility.ToString(consensusSeedPeer),
            });
        }
        else
        {
            return SeedInfo.Empty;
        }
    }
}
