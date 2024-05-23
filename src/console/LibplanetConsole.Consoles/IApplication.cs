using LibplanetConsole.Consoles.Serializations;

namespace LibplanetConsole.Consoles;

public interface IApplication : IAsyncDisposable, IServiceProvider
{
    ApplicationInfo Info { get; }

    Task InvokeAsync(Action action);

    Task<T> InvokeAsync<T>(Func<T> func);

    void Cancel();

    IClient GetClient(string address);

    INode GetNode(string address);

    IAddressable GetAddressable(string address);
}
