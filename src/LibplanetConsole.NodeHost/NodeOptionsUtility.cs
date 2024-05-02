using JSSoft.Communication;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.NodeServices;
using LibplanetConsole.NodeServices.Serializations;

namespace LibplanetConsole.NodeHost;

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

        try
        {
            var closeToken = await clientContext.OpenAsync(cancellationToken);
            var seedInfo = await clientService.Server.GetSeedAsync(cancellationToken);
            await clientContext.CloseAsync(closeToken, cancellationToken);
            return (NodeOptionsInfo)seedInfo;
        }
        catch
        {
            await clientContext.AbortAsync();
            throw;
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