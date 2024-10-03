using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Node.Services;

namespace LibplanetConsole.Client.Services;

[Export]
[Export(typeof(IRemoteService))]
internal sealed class RemoteNodeService(Client client)
    : RemoteService<INodeService, INodeCallback>(client)
{
}
