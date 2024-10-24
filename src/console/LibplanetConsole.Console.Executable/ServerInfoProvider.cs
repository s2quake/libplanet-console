using LibplanetConsole.Common;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace LibplanetConsole.Console.Executable;

internal sealed class ServerInfoProvider(IServer server)
    : InfoProviderBase<IHostApplicationLifetime>("Server")
{
    protected override object? GetInfo(IHostApplicationLifetime obj)
    {
        return new
        {
            Addresses = GetAddresses(server),
        };
    }

    private static string[] GetAddresses(IServer server)
    {
        if (server.Features.Get<IServerAddressesFeature>() is { } addressesFeature)
        {
            return [.. addressesFeature.Addresses];
        }

        return [];
    }
}
