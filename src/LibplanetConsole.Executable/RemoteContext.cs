using JSSoft.Communication;

namespace LibplanetConsole.Executable;

internal sealed class RemoteContext(params IService[] services)
    : ClientContext(services)
{
}
