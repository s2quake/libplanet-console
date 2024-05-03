using System.ComponentModel.Composition;
using JSSoft.Communication;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Serializations;
using LibplanetConsole.Frameworks;
using LibplanetConsole.NodeServices.Seeds;

namespace LibplanetConsole.Executable;

[Export(typeof(IService))]
[Export(typeof(IApplicationService))]
internal sealed class SeedService : ServerService<ISeedService>,
    ISeedService, IApplicationService
{
    private readonly IApplication _application;
    private readonly NodeCollection _nodes;
    private readonly PrivateKey _seedNodePrivateKey = new();
    private readonly SeedNode _blocksyncSeedNode;
    private readonly SeedNode _consensusSeedNode;

    [ImportingConstructor]
    public SeedService(IApplication application, NodeCollection nodes)
    {
        _application = application;
        _nodes = nodes;
        _blocksyncSeedNode = new SeedNode(new()
        {
            PrivateKey = _seedNodePrivateKey,
            EndPoint = DnsEndPointUtility.Next(),
            AppProtocolVersion = BlockChainUtility.AppProtocolVersion,
        });
        _consensusSeedNode = new SeedNode(new()
        {
            PrivateKey = _seedNodePrivateKey,
            EndPoint = DnsEndPointUtility.Next(),
            AppProtocolVersion = BlockChainUtility.AppProtocolVersion,
        });
    }

    public async Task<SeedInfo> GetSeedAsync(CancellationToken cancellationToken)
    {
        var seedPeer = _blocksyncSeedNode.BoundPeer;
        var consensusSeedPeer = _consensusSeedNode.BoundPeer;

        return await _application.InvokeAsync(() => new SeedInfo
        {
            GenesisOptions = _application.GenesisOptions,
            BlocksyncSeedPeer = BoundPeerUtility.ToString(seedPeer),
            ConsensusSeedPeer = BoundPeerUtility.ToString(consensusSeedPeer),
        });
    }

    public Task<string> GetNodeEndPointAsync(CancellationToken cancellationToken)
    {
        var node = _nodes.RandomNode();
        return _application.InvokeAsync(() => EndPointUtility.ToString(node.EndPoint));
    }

    async Task IApplicationService.InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        await _blocksyncSeedNode.StartAsync(cancellationToken);
        await _consensusSeedNode.StartAsync(cancellationToken);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _blocksyncSeedNode.StopAsync(cancellationToken: default);
        await _consensusSeedNode.StopAsync(cancellationToken: default);
    }
}
