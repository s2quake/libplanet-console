using JSSoft.Commands;

namespace LibplanetConsole.Client.Commands;

internal sealed class AddressCommand(IClient client) : CommandBase
{
    protected override void OnExecute() => Out.WriteLine(client.Address);
}
