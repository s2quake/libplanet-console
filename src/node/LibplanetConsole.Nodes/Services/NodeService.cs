using System.ComponentModel.Composition;
using JSSoft.Communication;

namespace LibplanetConsole.Nodes.Services;

[Export(typeof(IService))]
[method: ImportingConstructor]
internal sealed class NodeService(Node node)
    : NodeServiceBase(node)
{
}
