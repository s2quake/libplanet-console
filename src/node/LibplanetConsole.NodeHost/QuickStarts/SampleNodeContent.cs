using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.NodeHost.QuickStarts;

[Export(typeof(ISampleNodeContent))]
[Export(typeof(INodeContent))]
[Export]
[method: ImportingConstructor]
internal sealed class SampleNodeContent(IApplication application, INode node)
    : NodeContentBase(node), INodeContent, ISampleNodeContent
{
    private readonly IApplication _application = application;
    private readonly HashSet<Address> _addresses = [];

    public event EventHandler<ItemEventArgs<Address>>? Subscribed;

    public event EventHandler<ItemEventArgs<Address>>? Unsubscribed;

    public int Count => _addresses.Count;

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
