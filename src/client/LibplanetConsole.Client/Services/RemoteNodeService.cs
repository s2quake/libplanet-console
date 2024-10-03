using LibplanetConsole.Common.Services;
using LibplanetConsole.Node.Services;

namespace LibplanetConsole.Client.Services;

internal sealed class RemoteNodeService(Client client)
    : RemoteService<INodeService, INodeCallback>(client)
{
}
