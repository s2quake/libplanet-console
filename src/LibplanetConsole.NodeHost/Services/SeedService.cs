using System.ComponentModel.Composition;
using JSSoft.Communication;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Serializations;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.NodeHost.Services;

[Export(typeof(IService))]
[method: ImportingConstructor]
internal sealed class SeedService(IApplication application, INode node)
    : ServerService<ISeedService>, ISeedService
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
