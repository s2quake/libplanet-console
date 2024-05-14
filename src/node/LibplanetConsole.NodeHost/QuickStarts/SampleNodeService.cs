using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.QuickStarts;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.NodeHost.QuickStarts;

[Export(typeof(ILocalService))]
internal sealed class SampleNodeService : LocalService<ISampleNodeService, ISampleNodeCallbak>,
    ISampleNodeService, IDisposable
{
    private readonly SampleNodeContent _sampleNode;

    [ImportingConstructor]
    public SampleNodeService(SampleNodeContent sampleNode)
    {
        _sampleNode = sampleNode;
        _sampleNode.Subscribed += SampleNode_Subscribed;
        _sampleNode.Unsubscribed += SampleNode_Unsubscribed;
    }

    public void Dispose()
    {
        _sampleNode.Subscribed -= SampleNode_Subscribed;
        _sampleNode.Unsubscribed -= SampleNode_Unsubscribed;
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

    private void SampleNode_Subscribed(object? sender, ItemEventArgs<Address> e)
    {
        Callback.OnSubscribed(AddressUtility.ToString(e.Item));
    }

    private void SampleNode_Unsubscribed(object? sender, ItemEventArgs<Address> e)
    {
        Callback.OnUnsubscribed(AddressUtility.ToString(e.Item));
    }
}
