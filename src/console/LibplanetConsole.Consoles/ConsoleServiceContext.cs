using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Consoles;

[Export]
internal sealed class ConsoleServiceContext : LocalServiceContext
{
    [ImportingConstructor]
    public ConsoleServiceContext(
        [ImportMany] IEnumerable<ILocalService> localServices, ApplicationOptions options)
        : base([.. localServices])
    {
        EndPoint = DnsEndPointUtility.Parse(options.EndPoint);
    }
}
