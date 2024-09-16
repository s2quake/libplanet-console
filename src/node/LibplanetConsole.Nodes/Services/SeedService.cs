using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Seeds;

namespace LibplanetConsole.Nodes.Services;

[Export(typeof(ILocalService))]
[Export(typeof(IApplicationService))]
[method: ImportingConstructor]
internal sealed class SeedService(ApplicationBase application)
    : LocalService<ISeedService>, ISeedService, IApplicationService, IAsyncDisposable
{
    private readonly AppPrivateKey _seedNodePrivateKey = new();
    private Seed? _blocksyncSeed;
    private Seed? _consensusSeed;

    public async Task<SeedInfo> GetSeedAsync(
        AppPublicKey publicKey, CancellationToken cancellationToken)
    {
        if (_blocksyncSeed is null || _consensusSeed is null)
        {
            throw new InvalidOperationException("The SeedService is not running.");
        }

        var seedPeer = _blocksyncSeed.BoundPeer;
        var consensusSeedPeer = _consensusSeed.BoundPeer;
        var seedInfo = new SeedInfo
        {
            BlocksyncSeedPeer = seedPeer,
            ConsensusSeedPeer = consensusSeedPeer,
        };

        return await application.InvokeAsync(() => seedInfo, cancellationToken);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_blocksyncSeed is not null)
        {
            await _blocksyncSeed.StopAsync(cancellationToken: default);
            _blocksyncSeed = null;
        }

        if (_consensusSeed is not null)
        {
            await _consensusSeed.StopAsync(cancellationToken: default);
            _consensusSeed = null;
        }
    }

    async Task IApplicationService.InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        if (application.Info.IsSingleNode is true)
        {
            _blocksyncSeed = new Seed(new()
            {
                PrivateKey = _seedNodePrivateKey,
                EndPoint = AppEndPoint.Next(),
            });
            _consensusSeed = new Seed(new()
            {
                PrivateKey = _seedNodePrivateKey,
                EndPoint = AppEndPoint.Next(),
            });
            await _blocksyncSeed.StartAsync(cancellationToken);
            await _consensusSeed.StartAsync(cancellationToken);
        }
    }
}
