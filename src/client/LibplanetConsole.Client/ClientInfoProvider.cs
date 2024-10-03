using LibplanetConsole.Common;

namespace LibplanetConsole.Client;

internal sealed class ClientInfoProvider(Client client)
    : InfoProviderBase<ApplicationBase>(nameof(Client))
{
    protected override object? GetInfo(ApplicationBase obj)
    {
        return client.Info;
    }
}
