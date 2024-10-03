using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Node.Services;

internal sealed class NodeContext(IEnumerable<ILocalService> localServices)
    : LocalServiceContext(localServices)
{
}
