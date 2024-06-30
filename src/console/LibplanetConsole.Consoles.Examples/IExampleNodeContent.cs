using LibplanetConsole.Common;

namespace LibplanetConsole.Consoles.Examples;

public interface IExampleNodeContent
{
    event EventHandler<ItemEventArgs<AppAddress>>? Subscribed;

    event EventHandler<ItemEventArgs<AppAddress>>? Unsubscribed;

    int Count { get; }

    Task<AppAddress[]> GetAddressesAsync(CancellationToken cancellationToken);

    void Subscribe(AppAddress address);

    void Unsubscribe(AppAddress address);
}
