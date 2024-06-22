using System.Net;
using JSSoft.Communication;

namespace LibplanetConsole.Common.Services;

public class RemoteServiceContext
{
    private readonly InternalClientContext _clientContext;
    private AppEndPoint? _endPoint;

    public RemoteServiceContext(IEnumerable<IRemoteService> remoteServices)
    {
        _clientContext = new([.. remoteServices.Select(service => service.Service)]);
        _clientContext.Opened += (s, e) => Opened?.Invoke(this, EventArgs.Empty);
        _clientContext.Closed += (s, e) => Closed?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? Opened;

    public event EventHandler? Closed;

    public AppEndPoint EndPoint
    {
        get => _endPoint ?? throw new InvalidOperationException("EndPoint is not set.");
        set
        {
            _endPoint = value;
            _clientContext.EndPoint = (EndPoint)value;
        }
    }

    public bool IsRunning => _clientContext.ServiceState == ServiceState.Open;

    public async Task<Guid> OpenAsync(CancellationToken cancellationToken)
    {
        return await _clientContext.OpenAsync(cancellationToken);
    }

    public Task CloseAsync(Guid token)
        => CloseAsync(token, CancellationToken.None);

    public async Task CloseAsync(Guid token, CancellationToken cancellationToken)
    {
        if (_clientContext.ServiceState == ServiceState.Open)
        {
            try
            {
                await _clientContext.CloseAsync(token, cancellationToken);
            }
            catch
            {
                // Ignore.
            }
        }

        if (_clientContext.ServiceState == ServiceState.Faulted)
        {
            await _clientContext.AbortAsync();
        }
    }

    private sealed class InternalClientContext(IService[] services) : ClientContext(services)
    {
    }
}
