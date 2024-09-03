using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes.Examples;

[Export(typeof(IExampleNode))]
[Export]
[method: ImportingConstructor]
internal sealed class ExampleNode(
    IApplication application, ExampleNodeSettings settings) : IExampleNode
{
    private readonly IApplication _application = application;
    private readonly HashSet<AppAddress> _addresses = [];

    public event EventHandler<ItemEventArgs<AppAddress>>? Subscribed;

    public event EventHandler<ItemEventArgs<AppAddress>>? Unsubscribed;

    public int Count => _addresses.Count;

    public bool IsExample { get; } = settings.IsExample;

    public Task<AppAddress[]> GetAddressesAsync(CancellationToken cancellationToken)
        => _application.InvokeAsync(() => _addresses.ToArray(), cancellationToken);

    public void Subscribe(AppAddress address)
    {
        if (_addresses.Contains(address) == true)
        {
            throw new ArgumentException("Address is already subscribed.", nameof(address));
        }

        _addresses.Add(address);
        Subscribed?.Invoke(this, new ItemEventArgs<AppAddress>(address));
        Console.Out.WriteLine($"Subscribed: '{address}'");
    }

    public void Unsubscribe(AppAddress address)
    {
        if (_addresses.Contains(address) != true)
        {
            throw new ArgumentException("Address is not subscribed.", nameof(address));
        }

        _addresses.Remove(address);
        Unsubscribed?.Invoke(this, new ItemEventArgs<AppAddress>(address));
        Console.Out.WriteLine($"Unsubscribed: '{address}'");
    }
}
