using System.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Clients;

internal static class SeedUtility
{
    public static async Task<EndPoint> GetNodeEndPointAsync(
        EndPoint seedEndPoint, CancellationToken cancellationToken)
    {
        var remoteService = new RemoteService<ISeedService>();
        var remoteServiceContext = new RemoteServiceContext([remoteService])
        {
            EndPoint = seedEndPoint,
        };
        var closeToken = await remoteServiceContext.OpenAsync(cancellationToken);
        var nodeEndPoint = await remoteService.Service.GetNodeEndPointAsync(cancellationToken);
        await remoteServiceContext.CloseAsync(closeToken, cancellationToken);
        return EndPointUtility.Parse(nodeEndPoint);
    }

    public static async Task<EndPoint> GetNodeEndPointAsync(
        EndPoint nodEndPoint, bool isSeed, CancellationToken cancellationToken)
    {
        if (isSeed == true)
        {
            return await GetNodeEndPointAsync(nodEndPoint, cancellationToken);
        }

        return nodEndPoint;
    }
}
