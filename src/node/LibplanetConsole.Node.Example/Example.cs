using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Example;

internal sealed class Example(ExampleSettings settings) : IExample
{
    private readonly HashSet<Address> _addresses = [];

    public event EventHandler<ItemEventArgs<Address>>? Subscribed;

    public event EventHandler<ItemEventArgs<Address>>? Unsubscribed;

    public int Count => _addresses.Count;

    public bool IsExample { get; } = settings.IsExample;

    public Task<Address[]> GetAddressesAsync(CancellationToken cancellationToken)
        => Task.Run(() => _addresses.ToArray(), cancellationToken);

    public void Subscribe(Address address)
    {
        if (_addresses.Contains(address) == true)
        {
            throw new ArgumentException("Address is already subscribed.", nameof(address));
        }

        _addresses.Add(address);
        Subscribed?.Invoke(this, new ItemEventArgs<Address>(address));
        Console.Out.WriteLine($"Subscribed: '{address}'");
    }

    public void Unsubscribe(Address address)
    {
        if (_addresses.Contains(address) != true)
        {
            throw new ArgumentException("Address is not subscribed.", nameof(address));
        }

        _addresses.Remove(address);
        Unsubscribed?.Invoke(this, new ItemEventArgs<Address>(address));
        Console.Out.WriteLine($"Unsubscribed: '{address}'");
    }
}
