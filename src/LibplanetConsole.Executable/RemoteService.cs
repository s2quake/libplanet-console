using JSSoft.Communication;

namespace LibplanetConsole.Executable;

internal sealed class RemoteService<TServer, TClient> : ClientService<TServer, TClient>
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
