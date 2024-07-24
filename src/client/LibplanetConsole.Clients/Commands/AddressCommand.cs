using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace LibplanetConsole.Clients.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
internal sealed class AddressCommand(IClient client) : CommandBase
{
    protected override void OnExecute() => Out.WriteLine(client.Address);
}
