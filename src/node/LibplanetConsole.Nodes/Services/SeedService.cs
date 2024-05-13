using System.ComponentModel.Composition;
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

    public async Task<SeedInfo> GetSeedAsync(CancellationToken cancellationToken)
    {
        if (node.IsRunning == true)
        {
            var nodeInfo = node.Info;
            var blocksyncSeedPeer = node.BlocksyncSeedPeer;
            var consensusSeedPeer = node.ConsensusSeedPeer;

            return await application.InvokeAsync(() => new SeedInfo
            {
                GenesisOptions = node.NodeOptions.GenesisOptions,
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
