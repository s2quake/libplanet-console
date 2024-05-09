using JSSoft.Communication;
using JSSoft.Communication.Extensions;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Serializations;
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
        var clientService = new ClientService<ISeedService>();
        var clientContext = new ClientContext(clientService)
        {
            EndPoint = DnsEndPointUtility.GetEndPoint(seedEndPoint),
        };
        var closeToken = Guid.Empty;

        try
        {
            closeToken = await clientContext.OpenAsync(cancellationToken);
            for (var i = 0; i < 10; i++)
            {
                var seedInfo = await clientService.Server.GetSeedAsync(cancellationToken);
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
            await clientContext.ReleaseAsync(closeToken);
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
