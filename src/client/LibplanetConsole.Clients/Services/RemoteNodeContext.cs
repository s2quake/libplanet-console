using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Clients.Services;

[Export]
[method: ImportingConstructor]
internal sealed class RemoteNodeContext(
    [ImportMany] IEnumerable<IRemoteService> remoteServices)
        : RemoteServiceContext([.. remoteServices])
{
}
