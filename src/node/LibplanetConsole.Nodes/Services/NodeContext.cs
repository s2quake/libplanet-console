using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Nodes.Services;

[Export]
internal sealed class NodeContext : LocalServiceContext
{
    [ImportingConstructor]
    public NodeContext(
        [ImportMany] IEnumerable<ILocalService> localServices, ApplicationOptions options)
        : base(localServices)
    {
        EndPoint = EndPointUtility.Parse(options.EndPoint);
    }
}
