using JSSoft.Communication;
using LibplanetConsole.NodeServices;

namespace LibplanetConsole.ClientServices;

internal sealed class NodeRemoteService(ClientBase client)
    : ClientService<INodeService, INodeCallback>(client)
{
}
