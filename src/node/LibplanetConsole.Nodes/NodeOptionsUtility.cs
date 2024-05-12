using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Serializations;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Nodes.Serializations;

namespace LibplanetConsole.Nodes;

internal static class NodeOptionsUtility
{
    public static NodeOptions GetNodeOptions(INode node)
    {
        var genesisKey = node.PrivateKey;
        var genesisValidators = GetPublicKeys(genesisKey);
        return new NodeOptions
        {
            GenesisOptions = new GenesisOptions
            {
                GenesisKey = genesisKey,
                GenesisValidators = genesisValidators,
            },
        };
    }

    public static async Task<NodeOptions> GetNodeOptionsAsync(
        string seedEndPoint, CancellationToken cancellationToken)
    {
        var remoteService = new RemoteService<ISeedService>();
        var remoteServiceContext = new RemoteServiceContext([remoteService])
        {
            EndPoint = DnsEndPointUtility.Parse(seedEndPoint),
        };

        var closeToken = await remoteServiceContext.OpenAsync(cancellationToken);
        try
        {
            for (var i = 0; i < 10; i++)
            {
                var seedInfo = await remoteService.Service.GetSeedAsync(cancellationToken);
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
            await remoteServiceContext.CloseAsync(closeToken);
        }
    }

    private static PublicKey[] GetPublicKeys(PrivateKey privateKey)
    {
        if (Environment.GetEnvironmentVariable("LIBPLANET_CONSOLE_VALIDATORS") is { } values)
        {
            return [.. values.Split(';').Select(item => PrivateKeyUtility.Parse(item).PublicKey)];
        }

        return [privateKey.PublicKey];
    }
}
