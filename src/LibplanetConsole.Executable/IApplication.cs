using Libplanet.Crypto;
using LibplanetConsole.Common;

namespace LibplanetConsole.Executable;

public interface IApplication : IAsyncDisposable, IServiceProvider
{
    GenesisOptions GenesisOptions { get; }

    Task InvokeAsync(Action action);

    Task<T> InvokeAsync<T>(Func<T> func);

    void Cancel();

    IClient GetClient(string identifier);

    INode GetNode(string identifier);

    IIdentifier GetIdentifier(string identifier);

    IIdentifier GetIdentifier(Address address);
}
