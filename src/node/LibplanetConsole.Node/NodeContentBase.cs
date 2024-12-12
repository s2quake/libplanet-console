namespace LibplanetConsole.Node;

public abstract class NodeContentBase(string name) : INodeContent, IDisposable
{
    private readonly string _name = name;
    private bool _isDisposed;

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

    protected virtual Task OnStartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task OnStopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

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
