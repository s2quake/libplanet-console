namespace LibplanetConsole.Client;

public interface IApplication : IAsyncDisposable
{
    ApplicationInfo Info { get; }

    Task InvokeAsync(Action action, CancellationToken cancellationToken);

    Task<T> InvokeAsync<T>(Func<T> func, CancellationToken cancellationToken);

    void Cancel();
}
