using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Nodes.Services;

[Export]
[method: ImportingConstructor]
internal sealed class NodeContext(
    [ImportMany] IEnumerable<ILocalService> localServices) : LocalServiceContext(localServices)
{
}
