using System.ComponentModel.Composition;
using Libplanet.Crypto;
using Libplanet.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Serializations;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Seeds;

namespace LibplanetConsole.Consoles.Services;

[Export]
[Export(typeof(ILocalService))]
[Export(typeof(IApplicationService))]
internal sealed class SeedService : LocalService<ISeedService>,
    ISeedService, IApplicationService
{
    private readonly ApplicationBase _application;
    private readonly PrivateKey _seedNodePrivateKey = new();
    private readonly SeedNode _blocksyncSeedNode;
    private readonly SeedNode _consensusSeedNode;

    [ImportingConstructor]
    public SeedService(ApplicationBase application)
    {
        _application = application;
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

    public BoundPeer BlocksyncSeedPeer => _blocksyncSeedNode.BoundPeer;

    public BoundPeer ConsensusSeedPeer => _consensusSeedNode.BoundPeer;

    public async Task<SeedInfo> GetSeedAsync(
        PublicKey publicKey, CancellationToken cancellationToken)
    {
        var seedPeer = _blocksyncSeedNode.BoundPeer;
        var consensusSeedPeer = _consensusSeedNode.BoundPeer;
        var genesisOptions = (GenesisOptionsInfo)_application.GenesisOptions;
        var seedInfo = new SeedInfo
        {
            GenesisOptions = genesisOptions.Encrypt(publicKey),
            BlocksyncSeedPeer = BoundPeerUtility.ToString(seedPeer),
            ConsensusSeedPeer = BoundPeerUtility.ToString(consensusSeedPeer),
        };

        return await _application.InvokeAsync(() => seedInfo);
    }

    public Task<string> GetNodeEndPointAsync(CancellationToken cancellationToken)
    {
        if (_application.GetService(typeof(NodeCollection)) is NodeCollection nodes)
        {
            var node = nodes.RandomNode();
            return _application.InvokeAsync(() => EndPointUtility.ToString(node.EndPoint));
        }

        throw new InvalidOperationException("NodeCollection is not found.");
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
