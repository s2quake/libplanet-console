using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace LibplanetConsole.Node.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
internal sealed class AddressCommand(INode node) : CommandBase
{
    protected override void OnExecute() => Out.WriteLine(node.Address);
}
