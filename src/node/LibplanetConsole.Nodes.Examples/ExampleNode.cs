using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Nodes.Examples;

[Export(typeof(IExampleNode))]
[Export]
[method: ImportingConstructor]
internal sealed class ExampleNode(IApplication application) : IExampleNode
{
    private readonly IApplication _application = application;
    private readonly HashSet<Address> _addresses = [];

    public event EventHandler<ItemEventArgs<Address>>? Subscribed;

    public event EventHandler<ItemEventArgs<Address>>? Unsubscribed;

    public int Count => _addresses.Count;

    public bool IsExample { get; }
        = ApplicationSettingsParser.Peek<ExampleNodeSettings>().IsExample;

    public Task<Address[]> GetAddressesAsync(CancellationToken cancellationToken)
    {
        return _application.InvokeAsync(() => _addresses.ToArray());
    }

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