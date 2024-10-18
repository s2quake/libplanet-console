using JSSoft.Commands;

namespace LibplanetConsole.Node.Commands;

[CommandSummary("Prints the address of the node.")]
internal sealed class AddressCommand(INode node) : CommandBase
{
    protected override void OnExecute() => Out.WriteLine(node.Address);
}
