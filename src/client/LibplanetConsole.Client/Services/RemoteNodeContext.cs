using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Client.Services;

internal sealed class RemoteNodeContext(
    IEnumerable<IRemoteService> remoteServices)
        : RemoteServiceContext([.. remoteServices])
{
}
