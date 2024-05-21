namespace LibplanetConsole.Consoles;

public interface IApplication : IAsyncDisposable, IServiceProvider
{
    Task InvokeAsync(Action action);

    Task<T> InvokeAsync<T>(Func<T> func);

    void Cancel();
}
