using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Examples.Services;

namespace LibplanetConsole.Nodes.Examples;

[Export(typeof(ILocalService))]
internal sealed class ExampleNodeService : LocalService<IExampleNodeService, IExampleNodeCallbak>,
    IExampleNodeService, IDisposable
{
    private readonly ExampleNode _sampleNode;

    [ImportingConstructor]
    public ExampleNodeService(ExampleNode sampleNode)
    {
        _sampleNode = sampleNode;
        _sampleNode.Subscribed += ExampleNode_Subscribed;
        _sampleNode.Unsubscribed += ExampleNode_Unsubscribed;
    }

    public void Dispose()
    {
        _sampleNode.Subscribed -= ExampleNode_Subscribed;
        _sampleNode.Unsubscribed -= ExampleNode_Unsubscribed;
    }

    public async Task<int> GetAddressCountAsync(CancellationToken cancellationToken)
    {
        var addresses = await _sampleNode.GetAddressesAsync(cancellationToken);
        return addresses.Length;
    }

    public async Task<string[]> GetAddressesAsync(CancellationToken cancellationToken)
    {
        var addresses = await _sampleNode.GetAddressesAsync(cancellationToken);
        return [.. addresses.Select(AddressUtility.ToString)];
    }

    public void Subscribe(string address)
    {
        _sampleNode.Subscribe(AddressUtility.Parse(address));
    }

    public void Unsubscribe(string address)
    {
        _sampleNode.Unsubscribe(AddressUtility.Parse(address));
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
