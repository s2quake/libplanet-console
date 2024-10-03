using System.Text;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Example.Services;

namespace LibplanetConsole.Console.Example;

internal sealed class ExampleNode(INode node, ExampleSettings settings)
    : NodeContentBase(node), IExampleNodeCallback, IExampleNode, INodeContentService
{
    private readonly StringBuilder _log = new();
    private RemoteService<IExampleNodeService, IExampleNodeCallback>? _remoteService;

    public event EventHandler<ItemEventArgs<Address>>? Subscribed;

    public event EventHandler<ItemEventArgs<Address>>? Unsubscribed;

    public int Count { get; private set; }

    public bool IsExample { get; } = settings.IsNodeExample;

    IRemoteService INodeContentService.RemoteService => RemoteService;

    private IExampleNodeService Service => RemoteService.Service;

    private RemoteService<IExampleNodeService, IExampleNodeCallback> RemoteService
        => _remoteService ??= new RemoteService<IExampleNodeService, IExampleNodeCallback>(this);

    public Task<Address[]> GetAddressesAsync(CancellationToken cancellationToken)
        => Service.GetAddressesAsync(cancellationToken);

    public void Subscribe(Address address) => Service.Subscribe(address);

    public void Unsubscribe(Address address) => Service.Unsubscribe(address);

    async void IExampleNodeCallback.OnSubscribed(Address address)
    {
        Count = await Service.GetAddressCountAsync(CancellationToken.None);
        _log.AppendLine($"{nameof(IExampleNodeCallback.OnSubscribed)}: {address}");
        Subscribed?.Invoke(this, new ItemEventArgs<Address>(address));
    }

    async void IExampleNodeCallback.OnUnsubscribed(Address address)
    {
        Count = await Service.GetAddressCountAsync(CancellationToken.None);
        _log.AppendLine($"{nameof(IExampleNodeCallback.OnUnsubscribed)}: {address}");
        Unsubscribed?.Invoke(this, new ItemEventArgs<Address>(address));
    }
}
