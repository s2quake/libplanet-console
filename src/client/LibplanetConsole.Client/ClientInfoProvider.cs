using LibplanetConsole.Common;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Client;

internal sealed class ClientInfoProvider(Client client)
    : InfoProviderBase<IHostApplicationLifetime>(nameof(Client))
{
    protected override object? GetInfo(IHostApplicationLifetime obj)
    {
        return client.Info;
    }
}
