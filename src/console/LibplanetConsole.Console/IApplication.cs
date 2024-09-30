namespace LibplanetConsole.Console;

public interface IApplication : IAsyncDisposable, IServiceProvider
{
    ApplicationInfo Info { get; }

    Task InvokeAsync(Action action, CancellationToken cancellationToken);

    Task<T> InvokeAsync<T>(Func<T> func, CancellationToken cancellationToken);

    void Cancel();

    IClient GetClient(string address);

    INode GetNode(string address);

    IAddressable GetAddressable(string address);

    IAddressable GetAddressable(Address address)
        => GetAddressable(address.ToString());
}
