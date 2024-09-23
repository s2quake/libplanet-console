using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Clients.Services;

[Export]
[method: ImportingConstructor]
internal sealed class ClientServiceContext(
    [ImportMany] IEnumerable<ILocalService> localServices) : LocalServiceContext([.. localServices])
{
}
