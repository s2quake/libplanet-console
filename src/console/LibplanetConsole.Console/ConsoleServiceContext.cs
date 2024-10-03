using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Console;

internal sealed class ConsoleServiceContext(
    IEnumerable<ILocalService> localServices) : LocalServiceContext([.. localServices])
{
}
