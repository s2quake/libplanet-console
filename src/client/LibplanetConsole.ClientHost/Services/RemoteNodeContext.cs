using System.ComponentModel.Composition;
using LibplanetConsole.Clients.Services;

namespace LibplanetConsole.ClientHost.Services;

[Export]
[method: ImportingConstructor]
internal sealed class RemoteNodeContext(
    [ImportMany] IEnumerable<IRemoteNodeServiceProvider> serviceProviders)
    : RemoteNodeContextBase(serviceProviders)
{
}
