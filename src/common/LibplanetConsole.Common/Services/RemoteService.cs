// File may only contain a single type
#pragma warning disable SA1402
using JSSoft.Communication;

namespace LibplanetConsole.Common.Services;

public class RemoteService<TService, TCallback> : IRemoteService
    where TService : class
    where TCallback : class
{
    private readonly ClientService<TService, TCallback> _clientService;

    public RemoteService(TCallback callback)
    {
        _clientService = new ClientService<TService, TCallback>(callback);
    }

    public RemoteService()
    {
        var obj = this;
        if (obj is TCallback callback)
        {
            _clientService = new ClientService<TService, TCallback>(callback);
        }
        else
        {
            throw new InvalidOperationException(
                $"'{GetType()}' must be implemented by '{typeof(TCallback)}'.");
        }
    }

    public TService Service => _clientService.Server;

    IService IRemoteService.Service => _clientService;
}

public class RemoteService<TService> : IRemoteService
    where TService : class
{
    private readonly ClientService<TService> _clientService;

    public RemoteService()
    {
        _clientService = new ClientService<TService>();
    }

    public TService Service => _clientService.Server;

    IService IRemoteService.Service => _clientService;
}
