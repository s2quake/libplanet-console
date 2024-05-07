using System.ComponentModel.Composition;
using JSSoft.Communication;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.QuickStarts;

namespace LibplanetConsole.NodeHost.QuickStarts;

[Export(typeof(IService))]
internal sealed class SampleNodeService
    : ServerService<ISampleNodeService, ISampleNodeCallbak>, ISampleNodeService, IDisposable
{
    private readonly ISampleNode _sampleNode;

    [ImportingConstructor]
    public SampleNodeService(ISampleNode sampleNode)
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

    public int GetAddressCount() => _sampleNode.Count;

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
        Client.OnSubscribed(AddressUtility.ToString(e.Item));
    }

    private void SampleNode_Unsubscribed(object? sender, ItemEventArgs<Address> e)
    {
        Client.OnUnsubscribed(AddressUtility.ToString(e.Item));
    }
}
