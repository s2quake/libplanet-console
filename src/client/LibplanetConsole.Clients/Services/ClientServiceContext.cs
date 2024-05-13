using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Clients.Services;

[Export]
internal sealed class ClientServiceContext : LocalServiceContext
{
    [ImportingConstructor]
    public ClientServiceContext(
        [ImportMany] IEnumerable<ILocalService> localServices, ApplicationOptions options)
        : base([.. localServices])
    {
        EndPoint = EndPointUtility.Parse(options.EndPoint);
    }
}
