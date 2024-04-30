using System.Net;
using JSSoft.Communication;
using LibplanetConsole.Common;

namespace LibplanetConsole.ClientHost;

internal static class SeedUtility
{
    public static async Task<EndPoint> GetNodeEndPointAsync(
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
            var nodeEndPoint = await clientService.Server.GetNodeEndPointAsync(cancellationToken);
            await clientContext.CloseAsync(closeToken, cancellationToken);
            return EndPointUtility.Parse(nodeEndPoint);
        }
        catch
        {
            await clientContext.AbortAsync();
            throw;
        }
    }
}