using LibplanetConsole.Common;

namespace LibplanetConsole.Executable;

public interface IApplication : IAsyncDisposable, IServiceProvider
{
    GenesisOptions GenesisOptions { get; }

    Task InvokeAsync(Action action);

    Task<T> InvokeAsync<T>(Func<T> func);

    void Cancel();

    IClient GetClient(string address);

    INode GetNode(string address);

    IIdentifier GetIdentifier(string address);
}
