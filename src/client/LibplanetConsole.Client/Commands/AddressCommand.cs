using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Client.Commands;

[Export(typeof(ICommand))]
internal sealed class AddressCommand(IClient client) : CommandBase
{
    protected override void OnExecute() => Out.WriteLine(client.Address);
}
