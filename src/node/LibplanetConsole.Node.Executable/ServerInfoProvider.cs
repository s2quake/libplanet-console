using LibplanetConsole.Common;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Node.Executable;

internal sealed class ServerInfoProvider(IServer server)
    : InfoProviderBase<IHostApplicationLifetime>("Server")
{
    protected override object? GetInfo(IHostApplicationLifetime obj)
    {
        if (server.Features.Get<IServerAddressesFeature>() is { } addressesFeature)
        {

        }
        return new
        {
            Address = server.Features.Get<IServerAddressesFeature>().Addresses,
        };
    }
}
