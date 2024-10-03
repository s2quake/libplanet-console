using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Example;

public interface IExampleNode
{
    event EventHandler<ItemEventArgs<Address>>? Subscribed;

    event EventHandler<ItemEventArgs<Address>>? Unsubscribed;

    int Count { get; }

    Task<Address[]> GetAddressesAsync(CancellationToken cancellationToken);

    void Subscribe(Address address);

    void Unsubscribe(Address address);
}
