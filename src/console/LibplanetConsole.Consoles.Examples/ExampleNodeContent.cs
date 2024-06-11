using System.ComponentModel.Composition;
using System.Text;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Examples.Services;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles.Examples;

[Export(typeof(IExampleNodeContent))]
[Export(typeof(INodeContentService))]
[Export(typeof(INodeContent))]
internal sealed class ExampleNodeContent
    : NodeContentBase, IExampleNodeCallback, IExampleNodeContent, INodeContentService
{
    private readonly RemoteService<IExampleNodeService, IExampleNodeCallback> _remoteService;
    private readonly StringBuilder _log = new();

    [ImportingConstructor]
    public ExampleNodeContent(INode node)
        : base(node)
    {
        _remoteService = new RemoteService<IExampleNodeService, IExampleNodeCallback>(this);
    }

    public event EventHandler<ItemEventArgs<Address>>? Subscribed;

    public event EventHandler<ItemEventArgs<Address>>? Unsubscribed;

    public int Count { get; private set; }

    public bool IsExample { get; }
        = ApplicationSettingsParser.Peek<ExampleNodeSettings>().IsNodeExample;

    IRemoteService INodeContentService.RemoteService => _remoteService;

    private IExampleNodeService Service => _remoteService.Service;

    public async Task<Address[]> GetAddressesAsync(CancellationToken cancellationToken)
    {
        var addresses = await Service.GetAddressesAsync(cancellationToken);
        return [.. addresses.Select(item => AddressUtility.Parse(item))];
    }

    public void Subscribe(Address address)
        => Service.Subscribe(AddressUtility.ToString(address));

    public void Unsubscribe(Address address)
        => Service.Unsubscribe(AddressUtility.ToString(address));

    async void IExampleNodeCallback.OnSubscribed(string address)
    {
        Count = await Service.GetAddressCountAsync(CancellationToken.None);
        _log.AppendLine($"{nameof(IExampleNodeCallback.OnSubscribed)}: {address}");
        Subscribed?.Invoke(this, new ItemEventArgs<Address>(AddressUtility.Parse(address)));
    }

    async void IExampleNodeCallback.OnUnsubscribed(string address)
    {
        Count = await Service.GetAddressCountAsync(CancellationToken.None);
        _log.AppendLine($"{nameof(IExampleNodeCallback.OnUnsubscribed)}: {address}");
        Unsubscribed?.Invoke(this, new ItemEventArgs<Address>(AddressUtility.Parse(address)));
    }
}
