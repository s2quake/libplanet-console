using System.ComponentModel.Composition;
using System.Net;
using JSSoft.Communication;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes.Services;

[Export]
internal sealed class NodeContext : ServerContext
{
    [ImportingConstructor]
    public NodeContext(
        [ImportMany] IEnumerable<IService> services, ApplicationOptions options)
        : base([.. services])
    {
        EndPoint = GetEndPoint(options);
    }

    private static EndPoint GetEndPoint(ApplicationOptions options)
    {
        if (options.EndPoint != string.Empty)
        {
            return EndPointUtility.Parse(options.EndPoint);
        }

        return DnsEndPointUtility.Next();
    }
}
