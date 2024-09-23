using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Examples.Services;

namespace LibplanetConsole.Nodes.Examples;

[Export(typeof(ILocalService))]
internal sealed class ExampleNodeService : LocalService<IExampleNodeService, IExampleNodeCallback>,
    IExampleNodeService, IDisposable
{
    private readonly ExampleNode _exampleNode;

    [ImportingConstructor]
    public ExampleNodeService(ExampleNode exampleNode)
    {
        _exampleNode = exampleNode;
        _exampleNode.Subscribed += ExampleNode_Subscribed;
        _exampleNode.Unsubscribed += ExampleNode_Unsubscribed;
    }

    public void Dispose()
    {
        _exampleNode.Subscribed -= ExampleNode_Subscribed;
        _exampleNode.Unsubscribed -= ExampleNode_Unsubscribed;
    }

    public async Task<int> GetAddressCountAsync(CancellationToken cancellationToken)
    {
        var addresses = await _exampleNode.GetAddressesAsync(cancellationToken);
        return addresses.Length;
    }

    public Task<AppAddress[]> GetAddressesAsync(CancellationToken cancellationToken)
        => _exampleNode.GetAddressesAsync(cancellationToken);

    public void Subscribe(AppAddress address) => _exampleNode.Subscribe(address);

    public void Unsubscribe(AppAddress address) => _exampleNode.Unsubscribe(address);

    private void ExampleNode_Subscribed(object? sender, ItemEventArgs<AppAddress> e)
        => Callback.OnSubscribed(e.Item);

    private void ExampleNode_Unsubscribed(object? sender, ItemEventArgs<AppAddress> e)
        => Callback.OnUnsubscribed(e.Item);
}
