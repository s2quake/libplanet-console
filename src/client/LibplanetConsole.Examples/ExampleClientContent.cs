using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Clients;
using LibplanetConsole.Common;
using LibplanetConsole.Examples;

namespace LibplanetConsole.Examples;

[Export(typeof(IExampleClientContent))]
[method: ImportingConstructor]
internal sealed class ExampleClientContent(
    IClient client, ExampleRemoteNodeService remoteNodeService) : IExampleClientContent
{
    private readonly ExampleRemoteNodeService _remoteNodeService = remoteNodeService;

    public Address Address => client.Address;

    private IExampleNodeService Server => _remoteNodeService.Service;

    public void Subscribe()
    {
        Server.Subscribe(AddressUtility.ToString(Address));
    }

    public void Unsubscribe()
    {
        Server.Unsubscribe(AddressUtility.ToString(Address));
    }
}
