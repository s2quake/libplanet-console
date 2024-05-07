using LibplanetConsole.ClientHost.Serializations;

namespace LibplanetConsole.ClientHost;

public interface IApplication : IAsyncDisposable, IServiceProvider
{
    ApplicationInfo Info { get; }

    Task InvokeAsync(Action action);

    Task<T> InvokeAsync<T>(Func<T> func);

    void Cancel();
}
