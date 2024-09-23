using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace LibplanetConsole.Nodes.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
internal sealed class AddressCommand(INode node) : CommandBase
{
    protected override void OnExecute() => Out.WriteLine(node.Address);
}
