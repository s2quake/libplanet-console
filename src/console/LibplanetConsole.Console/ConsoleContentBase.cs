namespace LibplanetConsole.Console;

public abstract class ConsoleContentBase(string name) : IConsoleContent, IDisposable
{
    private readonly string _name = name;
    private bool _isDisposed;

    public string Name => _name != string.Empty ? _name : GetType().Name;

    public virtual IEnumerable<IConsoleContent> Dependencies => [];

    void IDisposable.Dispose()
    {
        OnDispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    Task IConsoleContent.StartAsync(CancellationToken cancellationToken)
        => OnStartAsync(cancellationToken);

    Task IConsoleContent.StopAsync(CancellationToken cancellationToken)
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
