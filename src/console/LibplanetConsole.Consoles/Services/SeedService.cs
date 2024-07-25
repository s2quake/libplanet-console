using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Seeds;

namespace LibplanetConsole.Consoles.Services;

[Export]
[Export(typeof(ILocalService))]
[Export(typeof(IApplicationService))]
internal sealed class SeedService : LocalService<ISeedService>,
    ISeedService, IApplicationService, IAsyncDisposable
{
    private readonly ApplicationBase _application;
    private readonly AppPrivateKey _seedNodePrivateKey = new();
    private readonly SeedNode _blocksyncSeedNode;
    private readonly SeedNode _consensusSeedNode;

    [ImportingConstructor]
    public SeedService(ApplicationBase application)
    {
        _application = application;
        _blocksyncSeedNode = new SeedNode(new()
        {
            PrivateKey = _seedNodePrivateKey,
            EndPoint = AppEndPoint.Next(),
        });
        _consensusSeedNode = new SeedNode(new()
        {
            PrivateKey = _seedNodePrivateKey,
            EndPoint = AppEndPoint.Next(),
        });
    }

    public AppPeer BlocksyncSeedPeer => _blocksyncSeedNode.BoundPeer;

    public AppPeer ConsensusSeedPeer => _consensusSeedNode.BoundPeer;

    public async Task<SeedInfo> GetSeedAsync(
        AppPublicKey publicKey, CancellationToken cancellationToken)
    {
        var seedPeer = _blocksyncSeedNode.BoundPeer;
        var consensusSeedPeer = _consensusSeedNode.BoundPeer;
        var genesisOptions = (GenesisInfo)_application.GenesisOptions;
        var seedInfo = new SeedInfo
        {
            GenesisInfo = genesisOptions.Encrypt(publicKey),
            BlocksyncSeedPeer = seedPeer,
            ConsensusSeedPeer = consensusSeedPeer,
        };

        return await _application.InvokeAsync(() => seedInfo, cancellationToken);
    }

    public Task<AppEndPoint> GetNodeEndPointAsync(CancellationToken cancellationToken)
    {
        if (_application.GetService(typeof(NodeCollection)) is NodeCollection nodes)
        {
            var node = nodes.RandomNode();
            return _application.InvokeAsync(Func, cancellationToken);

            AppEndPoint Func() => node.EndPoint;
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
