using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Nodes.Serializations;
using LibplanetConsole.Nodes.Services;

namespace LibplanetConsole.Clients.Services;

[Export(typeof(INodeService))]
[Export(typeof(IRemoteService))]
[method: ImportingConstructor]
internal sealed class RemoteNodeService(IApplication application)
    : RemoteService<INodeService, INodeCallback>, INodeCallback
{
    void INodeCallback.OnBlockAppended(BlockInfo blockInfo)
    {
        if (application.GetService(typeof(Client)) is Client client)
        {
            client.InvokeBlockAppendedEvent(blockInfo);
        }
        else
        {
            throw new InvalidOperationException("The client does not support.");
        }
    }
}
