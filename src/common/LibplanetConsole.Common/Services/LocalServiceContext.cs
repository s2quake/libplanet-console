using System.Net;
using JSSoft.Communication;

namespace LibplanetConsole.Common.Services;

public class LocalServiceContext
{
    private readonly ServerContext _serverContext;

    public LocalServiceContext(IEnumerable<ILocalService> localServices)
    {
        _serverContext = new ServerContext([.. localServices.Select(service => service.Service)]);
        _serverContext.Opened += (s, e) => Started?.Invoke(this, EventArgs.Empty);
        _serverContext.Closed += (s, e)
            => Stopped?.Invoke(this, new StopEventArgs(StopReason.None));
        _serverContext.Disconnected += (s, e)
            => Stopped?.Invoke(this, new StopEventArgs(StopReason.Disconnected));
        _serverContext.Faulted += (s, e)
            => Stopped?.Invoke(this, new StopEventArgs(StopReason.Faulted));
    }

    public event EventHandler? Started;

    public event EventHandler<StopEventArgs>? Stopped;

    public EndPoint EndPoint
    {
        get => _serverContext.EndPoint;
        set => _serverContext.EndPoint = value;
    }

    public bool IsRunning => _serverContext.ServiceState == ServiceState.Open;

    public async Task<Guid> StartAsync(CancellationToken cancellationToken)
    {
        return await _serverContext.OpenAsync(cancellationToken);
    }

    public Task StopAsync(Guid token)
        => CloseAsync(token, CancellationToken.None);

    public async Task CloseAsync(Guid token, CancellationToken cancellationToken)
    {
        if (_serverContext.ServiceState == ServiceState.Open)
        {
            try
            {
                await _serverContext.CloseAsync(token, cancellationToken);
            }
            catch
            {
            }
        }

        if (_serverContext.ServiceState == ServiceState.Faulted)
        {
            await _serverContext.AbortAsync();
        }
    }
}
