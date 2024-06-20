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
        => application.InvokeAsync(() => application.Info.EndPoint, cancellationToken);

    public Task<SeedInfo> GetSeedAsync(PublicKey publicKey, CancellationToken cancellationToken)
        => node.IsRunning switch
        {
            true => GetActualSeedAsync(publicKey, cancellationToken),
            _ => Task.Run(() => SeedInfo.Empty),
        };

    private Task<SeedInfo> GetActualSeedAsync(
        PublicKey publicKey, CancellationToken cancellationToken)
    {
        var blocksyncSeedPeer = node.BlocksyncSeedPeer;
        var consensusSeedPeer = node.ConsensusSeedPeer;
        var genesisOptions = (GenesisInfo)node.NodeOptions.GenesisOptions;

        return application.InvokeAsync(Func, cancellationToken);

        SeedInfo Func() => new()
        {
            GenesisInfo = genesisOptions.Encrypt(publicKey),
            BlocksyncSeedPeer = BoundPeerUtility.ToString(blocksyncSeedPeer),
            ConsensusSeedPeer = BoundPeerUtility.ToString(consensusSeedPeer),
        };
    }
}
