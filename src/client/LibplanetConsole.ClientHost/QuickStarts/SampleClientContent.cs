using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Clients;
using LibplanetConsole.Common;
using LibplanetConsole.Common.QuickStarts;

namespace LibplanetConsole.ClientHost.QuickStarts;

[Export(typeof(ISampleClientContent))]
[Export(typeof(IClientContent))]
[method: ImportingConstructor]
internal sealed class SampleClientContent(
    IClient client, SampleRemoteNodeService remoteNodeService)
    : ClientContentBase(client), ISampleClientContent
{
    private readonly SampleRemoteNodeService _remoteNodeService = remoteNodeService;

    public Address Address => Client.Address;

    private ISampleNodeService Server => _remoteNodeService.Service;

    public void Subscribe()
    {
        Server.Subscribe(AddressUtility.ToString(Address));
    }

    public void Unsubscribe()
    {
        Server.Unsubscribe(AddressUtility.ToString(Address));
    }
}
