using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Consoles;

[Export]
[method: ImportingConstructor]
internal sealed class ConsoleServiceContext(
    [ImportMany] IEnumerable<ILocalService> localServices) : LocalServiceContext([.. localServices])
{
}
