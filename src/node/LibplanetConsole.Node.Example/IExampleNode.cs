using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Example;

public interface IExampleNode
{
    event EventHandler<ItemEventArgs<AppAddress>>? Subscribed;

    event EventHandler<ItemEventArgs<AppAddress>>? Unsubscribed;

    int Count { get; }

    bool IsExample { get; }

    Task<AppAddress[]> GetAddressesAsync(CancellationToken cancellationToken);

    void Subscribe(AppAddress address);

    void Unsubscribe(AppAddress address);
}
