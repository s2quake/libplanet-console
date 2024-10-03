using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Console;

[Export]
internal sealed class ConsoleServiceContext(
    IEnumerable<ILocalService> localServices) : LocalServiceContext([.. localServices])
{
}
