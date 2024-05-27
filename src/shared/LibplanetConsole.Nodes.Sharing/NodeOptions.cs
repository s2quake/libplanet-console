using Libplanet.Crypto;
using Libplanet.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Serializations;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Nodes.Serializations;

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
    public static NodeOptions Default { get; } = new NodeOptions();

    public GenesisOptions GenesisOptions { get; init; } = new();

    public BoundPeer? BlocksyncSeedPeer { get; init; }

    public BoundPeer? ConsensusSeedPeer { get; init; }

    public static implicit operator NodeOptions(NodeOptionsInfo info)
    {
        return new NodeOptions
        {
            GenesisOptions = info.GenesisOptions,
            BlocksyncSeedPeer = BoundPeerUtility.GetSafeBoundPeer(info.BlocksyncSeedPeer),
            ConsensusSeedPeer = BoundPeerUtility.GetSafeBoundPeer(info.ConsensusSeedPeer),
        };
    }

    public static implicit operator NodeOptionsInfo(NodeOptions nodeOptions)
    {
        return new NodeOptionsInfo
        {
            GenesisOptions = nodeOptions.GenesisOptions,
            BlocksyncSeedPeer = BoundPeerUtility.ToSafeString(nodeOptions.BlocksyncSeedPeer),
            ConsensusSeedPeer = BoundPeerUtility.ToSafeString(nodeOptions.ConsensusSeedPeer),
        };
    }

    public static NodeOptions Create(PublicKey publicKey)
    {
        return new NodeOptions
        {
            GenesisOptions = new GenesisOptions()
            {
                GenesisKey = new PrivateKey(),
                GenesisValidators = [publicKey],
                Timestamp = DateTimeOffset.UtcNow,
            },
        };
    }

    public static async Task<NodeOptions> CreateAsync(
        string seedEndPoint, CancellationToken cancellationToken)
    {
        var remoteService = new RemoteService<ISeedService>();
        var remoteServiceContext = new RemoteServiceContext([remoteService])
        {
            EndPoint = DnsEndPointUtility.Parse(seedEndPoint),
        };
        var closeToken = await remoteServiceContext.OpenAsync(cancellationToken);
        var service = remoteService.Service;
        var privateKey = new PrivateKey();
        var publicKey = privateKey.PublicKey;
        try
        {
            for (var i = 0; i < 10; i++)
            {
                var decrypted = await service.GetSeedAsync(publicKey, cancellationToken);
                var seedInfo = decrypted.Decrypt(privateKey);
                if (Equals(seedInfo, SeedInfo.Empty) != true)
                {
                    return (NodeOptionsInfo)seedInfo;
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
