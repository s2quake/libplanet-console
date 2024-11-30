namespace LibplanetConsole.Client;

public abstract class ClientContentBase(string name) : IClientContent, IDisposable
{
    private readonly string _name = name;
    private bool _isDisposed;

    protected ClientContentBase()
        : this(string.Empty)
    {
    }

    public string Name => _name != string.Empty ? _name : GetType().Name;

    public virtual IEnumerable<IClientContent> Dependencies => [];

    void IDisposable.Dispose()
    {
        OnDispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    Task IClientContent.StartAsync(CancellationToken cancellationToken)
        => OnStartAsync(cancellationToken);

    Task IClientContent.StopAsync(CancellationToken cancellationToken)
        => OnStopAsync(cancellationToken);

    protected abstract Task OnStartAsync(CancellationToken cancellationToken);

    protected abstract Task OnStopAsync(CancellationToken cancellationToken);

    protected virtual void OnDispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // do nothing
            }

            _isDisposed = true;
        }
    }
}
