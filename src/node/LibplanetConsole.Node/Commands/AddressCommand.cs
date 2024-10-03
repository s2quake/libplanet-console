using JSSoft.Commands;

namespace LibplanetConsole.Node.Commands;

internal sealed class AddressCommand(INode node) : CommandBase
{
    protected override void OnExecute() => Out.WriteLine(node.Address);
}
