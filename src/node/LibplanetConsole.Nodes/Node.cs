using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes;

[Export]
[Export(typeof(INode))]
[method: ImportingConstructor]
internal sealed class Node(ApplicationOptions options)
    : NodeBase(
        privateKey: PrivateKeyUtility.Parse(options.PrivateKey),
        storePath: options.StorePath), INode
{
}
