using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Client;

[Export(typeof(IInfoProvider))]
[method: ImportingConstructor]
internal sealed class ClientInfoProvider(Client client)
    : InfoProviderBase<ApplicationBase>(nameof(Client))
{
    protected override object? GetInfo(ApplicationBase obj)
    {
        return client.Info;
    }
}
