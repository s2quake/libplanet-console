using System.ComponentModel.Composition;
using Libplanet.Crypto;
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

    public async Task<string[]> GetAddressesAsync(CancellationToken cancellationToken)
    {
        var addresses = await _exampleNode.GetAddressesAsync(cancellationToken);
        return [.. addresses.Select(AddressUtility.ToString)];
    }

    public void Subscribe(string address)
    {
        _exampleNode.Subscribe(AddressUtility.Parse(address));
    }

    public void Unsubscribe(string address)
    {
        _exampleNode.Unsubscribe(AddressUtility.Parse(address));
    }

    private void ExampleNode_Subscribed(object? sender, ItemEventArgs<Address> e)
    {
        Callback.OnSubscribed(AddressUtility.ToString(e.Item));
    }

    private void ExampleNode_Unsubscribed(object? sender, ItemEventArgs<Address> e)
    {
        Callback.OnUnsubscribed(AddressUtility.ToString(e.Item));
    }
}
