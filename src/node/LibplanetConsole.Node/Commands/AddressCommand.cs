using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Commands;

[Export(typeof(ICommand))]
internal sealed class AddressCommand(INode node) : CommandBase
{
    protected override void OnExecute() => Out.WriteLine(node.Address);
}
