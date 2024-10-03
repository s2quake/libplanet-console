using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Client.Services;

[Export]
internal sealed class RemoteNodeContext(
    IEnumerable<IRemoteService> remoteServices)
        : RemoteServiceContext([.. remoteServices])
{
}
