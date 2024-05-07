// File may only contain a single type
#pragma warning disable SA1402
using JSSoft.Communication;

namespace LibplanetConsole.Clients.Services;

public class RemoteNodeService<TServer, TClient> : ClientService<TServer, TClient>
    where TServer : class
    where TClient : class
{
    public RemoteNodeService()
    {
    }

    public RemoteNodeService(TClient callback)
        : base(callback)
    {
    }
}

public class RemoteNodeService<TServer> : ClientService<TServer>
    where TServer : class
{
    public RemoteNodeService()
    {
    }
}
