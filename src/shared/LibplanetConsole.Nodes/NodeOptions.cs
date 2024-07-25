using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;

#if LIBPLANET_CONSOLE
namespace LibplanetConsole.Consoles;
#elif LIBPLANET_NODE
namespace LibplanetConsole.Nodes;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Clients;
#else
#error "Either LIBPLANET_CONSOLE or LIBPLANET_NODE must be defined."
#endif

public sealed record class NodeOptions
{
    public required GenesisOptions GenesisOptions { get; init; }

    public AppPeer? BlocksyncSeedPeer { get; init; }

    public AppPeer? ConsensusSeedPeer { get; init; }

    public static implicit operator NodeOptions(SeedInfo seedInfo)
    {
        return new NodeOptions
        {
            GenesisOptions = seedInfo.GenesisInfo,
            BlocksyncSeedPeer = seedInfo.BlocksyncSeedPeer,
            ConsensusSeedPeer = seedInfo.ConsensusSeedPeer,
        };
    }

    public static NodeOptions Create(AppPublicKey publicKey)
    {
        return new NodeOptions
        {
            GenesisOptions = new GenesisOptions()
            {
                GenesisKey = new AppPrivateKey(),
                GenesisValidators = [publicKey],
                Timestamp = DateTimeOffset.UtcNow,
            },
        };
    }

    public static async Task<NodeOptions> CreateAsync(
        AppEndPoint seedEndPoint, CancellationToken cancellationToken)
    {
        var remoteService = new RemoteService<ISeedService>();
        var remoteServiceContext = new RemoteServiceContext([remoteService])
        {
            EndPoint = seedEndPoint,
        };
        var closeToken = await remoteServiceContext.OpenAsync(cancellationToken);
        var service = remoteService.Service;
        var privateKey = new AppPrivateKey();
        var publicKey = privateKey.PublicKey;
        try
        {
            for (var i = 0; i < 10; i++)
            {
                var decrypted = await service.GetSeedAsync(publicKey, cancellationToken);
                var seedInfo = decrypted.Decrypt(privateKey);
                if (Equals(seedInfo, SeedInfo.Empty) != true)
                {
                    return (NodeOptions)seedInfo;
                }

                await Task.Delay(500, cancellationToken);
            }

            throw new InvalidOperationException("No seed information is available.");
        }
        finally
        {
            await remoteServiceContext.CloseAsync(closeToken, cancellationToken);
        }
    }
}
