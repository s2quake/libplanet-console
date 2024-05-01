using System.ComponentModel.Composition;
using JSSoft.Communication;
using LibplanetConsole.NodeServices;

namespace LibplanetConsole.NodeHost;

[Export(typeof(IService))]
[method: ImportingConstructor]
internal sealed class NodeService(Node node)
    : NodeServiceBase(node)
{
}
