using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Console;

[Export]
[method: ImportingConstructor]
internal sealed class ConsoleServiceContext(
    [ImportMany] IEnumerable<ILocalService> localServices) : LocalServiceContext([.. localServices])
{
}
