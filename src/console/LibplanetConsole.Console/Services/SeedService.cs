using System.ComponentModel.Composition;
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
    private readonly AppPrivateKey _seedNodePrivateKey = new();
    private readonly Seed.Seed _blocksyncSeed;
    private readonly Seed.Seed _consensusSeed;

    [ImportingConstructor]
    public SeedService(ApplicationBase application)
    {
        _application = application;
        _blocksyncSeed = new Seed.Seed(new()
        {
            PrivateKey = _seedNodePrivateKey,
            EndPoint = AppEndPoint.Next(),
        });
        _consensusSeed = new Seed.Seed(new()
        {
            PrivateKey = _seedNodePrivateKey,
            EndPoint = AppEndPoint.Next(),
        });
    }

    public AppPeer BlocksyncSeedPeer => _blocksyncSeed.BoundPeer;

    public AppPeer ConsensusSeedPeer => _consensusSeed.BoundPeer;

    public async Task<SeedInfo> GetSeedAsync(
        AppPublicKey publicKey, CancellationToken cancellationToken)
    {
        var seedPeer = _blocksyncSeed.BoundPeer;
        var consensusSeedPeer = _consensusSeed.BoundPeer;
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
        await _blocksyncSeed.StartAsync(cancellationToken);
        await _consensusSeed.StartAsync(cancellationToken);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _blocksyncSeed.StopAsync(cancellationToken: default);
        await _consensusSeed.StopAsync(cancellationToken: default);
    }
}