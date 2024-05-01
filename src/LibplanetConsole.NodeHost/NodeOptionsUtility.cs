using JSSoft.Communication;
using LibplanetConsole.Common;
using LibplanetConsole.NodeServices;
using LibplanetConsole.NodeServices.Serializations;

namespace LibplanetConsole.NodeHost;

internal static class NodeOptionsUtility
{
    public static NodeOptions GetNodeOptions(INode node)
    {
        return new NodeOptions
        {
            GenesisOptions = new GenesisOptions
            {
                GenesisKey = node.PrivateKey,
                GenesisValidators = [node.PrivateKey.PublicKey],
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
}