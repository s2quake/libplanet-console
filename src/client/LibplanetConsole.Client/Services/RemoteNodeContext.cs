using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Client.Services;

[Export]
[method: ImportingConstructor]
internal sealed class RemoteNodeContext(
    [ImportMany] IEnumerable<IRemoteService> remoteServices)
        : RemoteServiceContext([.. remoteServices])
{
}
