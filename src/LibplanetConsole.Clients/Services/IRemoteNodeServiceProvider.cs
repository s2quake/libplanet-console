using JSSoft.Communication;

namespace LibplanetConsole.Clients.Services;

public interface IRemoteNodeServiceProvider
{
    IService Service { get; }
}
