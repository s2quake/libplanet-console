// File may only contain a single type
#pragma warning disable SA1402
using JSSoft.Communication;

namespace LibplanetConsole.Consoles.Services;

public sealed class RemoteService<TServer, TClient> : ClientService<TServer, TClient>
    where TServer : class
    where TClient : class
{
    public RemoteService()
    {
    }

    public RemoteService(TClient callback)
        : base(callback)
    {
    }
}

public sealed class RemoteService<TServer> : ClientService<TServer>
    where TServer : class
{
    public RemoteService()
    {
    }
}
