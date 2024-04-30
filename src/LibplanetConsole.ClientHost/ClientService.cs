using System.ComponentModel.Composition;
using JSSoft.Communication;
using LibplanetConsole.ClientServices;

namespace LibplanetConsole.ClientHost;

[Export(typeof(IService))]
[method: ImportingConstructor]
internal sealed class ClientService(Client client)
    : ClientServiceBase(client)
{
}
