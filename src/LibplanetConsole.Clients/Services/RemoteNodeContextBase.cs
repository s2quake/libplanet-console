using JSSoft.Communication;

namespace LibplanetConsole.Clients.Services;

public abstract class RemoteNodeContextBase(
    IEnumerable<IRemoteNodeServiceProvider> serviceProviders)
    : ClientContext(GetServices(serviceProviders)), IRemoteNodeContext
{
    private static IService[] GetServices(IEnumerable<IRemoteNodeServiceProvider> serviceProviders)
        => [.. serviceProviders.Select(item => item.Service)];
}
