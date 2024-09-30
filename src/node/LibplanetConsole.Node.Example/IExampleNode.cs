using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Example;

public interface IExampleNode
{
    event EventHandler<ItemEventArgs<Address>>? Subscribed;

    event EventHandler<ItemEventArgs<Address>>? Unsubscribed;

    int Count { get; }

    bool IsExample { get; }

    Task<Address[]> GetAddressesAsync(CancellationToken cancellationToken);

    void Subscribe(Address address);

    void Unsubscribe(Address address);
}
