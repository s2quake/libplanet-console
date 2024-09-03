using System.ComponentModel.Composition;
using System.Text;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Examples.Services;

namespace LibplanetConsole.Consoles.Examples;

[Export(typeof(IExampleNodeContent))]
[Export(typeof(INodeContentService))]
[Export(typeof(INodeContent))]
[method: ImportingConstructor]
internal sealed class ExampleNodeContent(INode node, ExampleNodeSettings settings)
    : NodeContentBase(node), IExampleNodeCallback, IExampleNodeContent, INodeContentService
{
    private readonly StringBuilder _log = new();
    private RemoteService<IExampleNodeService, IExampleNodeCallback>? _remoteService;

    public event EventHandler<ItemEventArgs<AppAddress>>? Subscribed;

    public event EventHandler<ItemEventArgs<AppAddress>>? Unsubscribed;

    public int Count { get; private set; }

    public bool IsExample { get; } = settings.IsNodeExample;

    IRemoteService INodeContentService.RemoteService => RemoteService;

    private IExampleNodeService Service => RemoteService.Service;

    private RemoteService<IExampleNodeService, IExampleNodeCallback> RemoteService
        => _remoteService ??= new RemoteService<IExampleNodeService, IExampleNodeCallback>(this);

    public Task<AppAddress[]> GetAddressesAsync(CancellationToken cancellationToken)
        => Service.GetAddressesAsync(cancellationToken);

    public void Subscribe(AppAddress address) => Service.Subscribe(address);

    public void Unsubscribe(AppAddress address) => Service.Unsubscribe(address);

    async void IExampleNodeCallback.OnSubscribed(AppAddress address)
    {
        Count = await Service.GetAddressCountAsync(CancellationToken.None);
        _log.AppendLine($"{nameof(IExampleNodeCallback.OnSubscribed)}: {address}");
        Subscribed?.Invoke(this, new ItemEventArgs<AppAddress>(address));
    }

    async void IExampleNodeCallback.OnUnsubscribed(AppAddress address)
    {
        Count = await Service.GetAddressCountAsync(CancellationToken.None);
        _log.AppendLine($"{nameof(IExampleNodeCallback.OnUnsubscribed)}: {address}");
        Unsubscribed?.Invoke(this, new ItemEventArgs<AppAddress>(address));
    }
}
