using LibplanetConsole.NodeHost.Serializations;

namespace LibplanetConsole.NodeHost;

public interface IApplication : IAsyncDisposable, IServiceProvider
{
    ApplicationInfo Info { get; }

    Task InvokeAsync(Action action);

    Task<T> InvokeAsync<T>(Func<T> func);

    void Cancel();
}
