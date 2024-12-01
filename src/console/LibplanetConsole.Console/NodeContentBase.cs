namespace LibplanetConsole.Console;

public abstract class NodeContentBase(string name) : INodeContent, IDisposable
{
    private readonly string _name = name;
    private bool _isDisposed;

    protected NodeContentBase()
        : this(string.Empty)
    {
    }

    public string Name => _name != string.Empty ? _name : GetType().Name;

    public virtual IEnumerable<INodeContent> Dependencies => [];

    void IDisposable.Dispose()
    {
        OnDispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    Task INodeContent.StartAsync(CancellationToken cancellationToken)
        => OnStartAsync(cancellationToken);

    Task INodeContent.StopAsync(CancellationToken cancellationToken)
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
