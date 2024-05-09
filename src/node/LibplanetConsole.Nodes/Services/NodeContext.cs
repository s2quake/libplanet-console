using System.ComponentModel.Composition;
using JSSoft.Communication;

namespace LibplanetConsole.Nodes.Services;

[Export]
internal sealed class NodeContext : ServerContext
{
    [ImportingConstructor]
    public NodeContext(
        [ImportMany] IEnumerable<IService> services, ApplicationOptions options)
        : base([.. services])
    {
        EndPoint = EndPointUtility.Parse(options.EndPoint);
    }
}
