using System.ComponentModel.Composition;
using System.Text;
using JSSoft.Communication;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.QuickStarts;

namespace LibplanetConsole.Executable.QuickStarts;

[Export(typeof(ISampleNode))]
[Export(typeof(INodeContent))]
[Export(typeof(ISampleNode))]
internal sealed class SampleNodeContent
    : NodeContentBase, ISampleNodeCallbak, IRemoteServiceProvider, ISampleNode
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

    public int Count => Server.GetAddressCount();

    private ISampleNodeService Server => _service.Server;

    public async Task<Address[]> GetAddressesAsync(CancellationToken cancellationToken)
    {
        var addresses = await Server.GetAddressesAsync(cancellationToken);
        return [.. addresses.Select(item => AddressUtility.Parse(item))];
    }

    public void Subscribe(Address address)
        => Server.Subscribe(AddressUtility.ToString(address));

    public void Unsubscribe(Address address)
        => Server.Unsubscribe(AddressUtility.ToString(address));

    void ISampleNodeCallbak.OnSubscribed(string address)
    {
        _log.AppendLine($"{nameof(ISampleNodeCallbak.OnSubscribed)}: {address}");
        Subscribed?.Invoke(this, new ItemEventArgs<Address>(AddressUtility.Parse(address)));
    }

    void ISampleNodeCallbak.OnUnsubscribed(string address)
    {
        _log.AppendLine($"{nameof(ISampleNodeCallbak.OnUnsubscribed)}: {address}");
        Unsubscribed?.Invoke(this, new ItemEventArgs<Address>(AddressUtility.Parse(address)));
    }

    IService IRemoteServiceProvider.GetService(object obj) => _service;
}
