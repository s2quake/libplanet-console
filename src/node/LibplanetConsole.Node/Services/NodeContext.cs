using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Node.Services;

[Export]
internal sealed class NodeContext(
    IEnumerable<ILocalService> localServices) : LocalServiceContext(localServices)
{
}
