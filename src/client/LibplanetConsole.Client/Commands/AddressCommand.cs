using JSSoft.Commands;

namespace LibplanetConsole.Client.Commands;

[CommandSummary("Prints the address of the client")]
internal sealed class AddressCommand(IClient client) : CommandBase
{
    protected override void OnExecute() => Out.WriteLine(client.Address);
}
