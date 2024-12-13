namespace LibplanetConsole.Console;

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

    public bool IsRunning { get; protected set; }

    void IDisposable.Dispose()
    {
        OnDispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    async Task IClientContent.StartAsync(CancellationToken cancellationToken)
    {
        await OnStartAsync(cancellationToken);
        IsRunning = true;
    }

    async Task IClientContent.StopAsync(CancellationToken cancellationToken)
    {
        await OnStopAsync(cancellationToken);
        IsRunning = false;
    }

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
