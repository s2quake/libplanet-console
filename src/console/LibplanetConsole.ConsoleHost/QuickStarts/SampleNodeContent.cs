using System.ComponentModel.Composition;
using System.Text;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.QuickStarts;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles;

namespace LibplanetConsole.ConsoleHost.QuickStarts;

[Export(typeof(ISampleNodeContent))]
[Export(typeof(INodeContent))]
[Export(typeof(ISampleNodeContent))]
internal sealed class SampleNodeContent
    : NodeContentBase, ISampleNodeCallbak, ISampleNodeContent
{
    private readonly RemoteService<ISampleNodeService, ISampleNodeCallbak> _service;
    private readonly StringBuilder _log = new();

    [ImportingConstructor]
    public SampleNodeContent(INode node)
        : base(node)
    {
        _service = new RemoteService<ISampleNodeService, ISampleNodeCallbak>(this);
    }

    public event EventHandler<ItemEventArgs<Address>>? Subscribed;

    public event EventHandler<ItemEventArgs<Address>>? Unsubscribed;

    public int Count { get; private set; }

    private ISampleNodeService Service => _service.Service;

    public async Task<Address[]> GetAddressesAsync(CancellationToken cancellationToken)
    {
        var addresses = await Service.GetAddressesAsync(cancellationToken);
        return [.. addresses.Select(item => AddressUtility.Parse(item))];
    }

    public void Subscribe(Address address)
        => Service.Subscribe(AddressUtility.ToString(address));

    public void Unsubscribe(Address address)
        => Service.Unsubscribe(AddressUtility.ToString(address));

    async void ISampleNodeCallbak.OnSubscribed(string address)
    {
        Count = await Service.GetAddressCountAsync(CancellationToken.None);
        _log.AppendLine($"{nameof(ISampleNodeCallbak.OnSubscribed)}: {address}");
        Subscribed?.Invoke(this, new ItemEventArgs<Address>(AddressUtility.Parse(address)));
    }

    async void ISampleNodeCallbak.OnUnsubscribed(string address)
    {
        Count = await Service.GetAddressCountAsync(CancellationToken.None);
        _log.AppendLine($"{nameof(ISampleNodeCallbak.OnUnsubscribed)}: {address}");
        Unsubscribed?.Invoke(this, new ItemEventArgs<Address>(AddressUtility.Parse(address)));
    }
}
