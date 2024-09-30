using System.ComponentModel.Composition;
using Libplanet.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Framework;
using LibplanetConsole.Seed;

namespace LibplanetConsole.Console.Services;

[Export]
[Export(typeof(ILocalService))]
[Export(typeof(IApplicationService))]
internal sealed class SeedService : LocalService<ISeedService>,
    ISeedService, IApplicationService, IAsyncDisposable
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
            EndPoint = AppEndPoint.Next(),
        });
        _consensusSeedNode = new SeedNode(new()
        {
            PrivateKey = _seedNodePrivateKey,
            EndPoint = AppEndPoint.Next(),
        });
    }

    public BoundPeer BlocksyncSeedPeer => _blocksyncSeedNode.BoundPeer;

    public BoundPeer ConsensusSeedPeer => _consensusSeedNode.BoundPeer;

    public async Task<SeedInfo> GetSeedAsync(
        PublicKey publicKey, CancellationToken cancellationToken)
    {
        var seedPeer = _blocksyncSeedNode.BoundPeer;
        var consensusSeedPeer = _consensusSeedNode.BoundPeer;
        var seedInfo = new SeedInfo
        {
            BlocksyncSeedPeer = seedPeer,
            ConsensusSeedPeer = consensusSeedPeer,
        };

        return await _application.InvokeAsync(() => seedInfo, cancellationToken);
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
