using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Nodes.Services;

namespace LibplanetConsole.Clients.Services;

[Export]
[Export(typeof(IRemoteService))]
[method: ImportingConstructor]
internal sealed class RemoteNodeService(Client client)
    : RemoteService<INodeService, INodeCallback>(client)
{
}
