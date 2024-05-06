using System.ComponentModel.Composition;
using JSSoft.Communication;
using LibplanetConsole.Nodes.Services;

namespace LibplanetConsole.NodeHost.Services;

[Export(typeof(IService))]
[method: ImportingConstructor]
internal sealed class NodeService(Node node)
    : NodeServiceBase(node)
{
}
