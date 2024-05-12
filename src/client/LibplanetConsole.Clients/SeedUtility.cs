using System.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Clients;

internal static class SeedUtility
{
    public static async Task<EndPoint> GetNodeEndPointAsync(
        string seedEndPoint, CancellationToken cancellationToken)
    {
        var remoteService = new RemoteService<ISeedService>();
        var remoteServiceContext = new RemoteServiceContext([remoteService])
        {
            EndPoint = EndPointUtility.Parse(seedEndPoint),
        };

        var closeToken = await remoteServiceContext.OpenAsync(cancellationToken);
        var nodeEndPoint = await remoteService.Service.GetNodeEndPointAsync(cancellationToken);
        await remoteServiceContext.CloseAsync(closeToken, cancellationToken);
        return EndPointUtility.Parse(nodeEndPoint);
    }
}