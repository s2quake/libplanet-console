using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Examples.Services;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Clients.Examples;

[Export(typeof(IExampleClient))]
[Export]
[method: ImportingConstructor]
internal sealed class ExampleClient(
    IClient client, ExampleRemoteNodeService remoteNodeService) : IExampleClient
{
    private readonly ExampleRemoteNodeService _remoteNodeService = remoteNodeService;

    public AppAddress Address => client.Address;

    public bool IsExample { get; }
        = ApplicationSettingsParser.Peek<ExampleClientSettings>().IsExample;

    private IExampleNodeService Server => _remoteNodeService.Service;

    public void Subscribe() => Server.Subscribe(Address);

    public void Unsubscribe() => Server.Unsubscribe(Address);
}
