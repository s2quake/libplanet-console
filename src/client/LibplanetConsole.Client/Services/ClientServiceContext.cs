using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Client.Services;

[Export]
internal sealed class ClientServiceContext(
    IEnumerable<ILocalService> localServices) : LocalServiceContext([.. localServices])
{
}
