using System.ComponentModel.Composition;
using JSSoft.Communication;
using LibplanetConsole.Clients.Services;
using LibplanetConsole.Nodes.Services;

namespace LibplanetConsole.ClientHost.Services;

[Export(typeof(IRemoteNodeServiceProvider))]
[method: ImportingConstructor]
internal sealed class RemoteNodeService(INodeCallback nodeCallback)
    : RemoteNodeService<INodeService, INodeCallback>(nodeCallback),
    IRemoteNodeServiceProvider
{
    IService IRemoteNodeServiceProvider.Service => this;
}
