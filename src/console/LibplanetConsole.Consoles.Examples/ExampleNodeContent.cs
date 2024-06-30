using System.ComponentModel.Composition;
using System.Text;
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

    public event EventHandler<ItemEventArgs<AppAddress>>? Subscribed;

    public event EventHandler<ItemEventArgs<AppAddress>>? Unsubscribed;

    public int Count { get; private set; }

    public bool IsExample { get; }
        = ApplicationSettingsParser.Peek<ExampleNodeSettings>().IsNodeExample;

    IRemoteService INodeContentService.RemoteService => _remoteService;

    private IExampleNodeService Service => _remoteService.Service;

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
