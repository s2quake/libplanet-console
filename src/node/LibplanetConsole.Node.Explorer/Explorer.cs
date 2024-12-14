using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace LibplanetConsole.Node.Explorer;

internal sealed class Explorer(IServer server) : NodeContentBase("explorer"), IExplorer
{
    public Uri Url => GetUrl(server);

    private static Uri GetUrl(IServer server)
    {
        if (server.Features.Get<IServerAddressesFeature>() is { } addressesFeature)
        {
            return new Uri($"{addressesFeature.Addresses.Last()}/ui/playground");
        }

        throw new InvalidOperationException("Server address is not found.");
    }
}
