using System.ComponentModel.Composition;
using JSSoft.Communication;
using LibplanetConsole.Clients.Services;

namespace LibplanetConsole.ClientHost.Services;

[Export(typeof(IService))]
[method: ImportingConstructor]
internal sealed class ClientService(Client client)
    : ClientServiceBase(client)
{
}
