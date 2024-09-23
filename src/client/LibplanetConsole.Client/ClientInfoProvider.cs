using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Clients;

[Export(typeof(IInfoProvider))]
[method: ImportingConstructor]
internal sealed class ClientInfoProvider(Client client) : InfoProviderBase<ApplicationBase>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(ApplicationBase obj)
    {
        yield return (nameof(Client), client.Info);
    }
}
