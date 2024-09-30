using System.ComponentModel.Composition;
using LibplanetConsole.Example.Services;

namespace LibplanetConsole.Client.Example;

[Export(typeof(IExampleClient))]
[Export]
[method: ImportingConstructor]
internal sealed class ExampleClient(
    IClient client,
    ExampleRemoteNodeService remoteNodeService,
    ExampleClientSettings settings)
    : IExampleClient
{
    private readonly ExampleRemoteNodeService _remoteNodeService = remoteNodeService;

    public Address Address => client.Address;

    public bool IsExample { get; } = settings.IsExample;

    private IExampleNodeService Server => _remoteNodeService.Service;

    public void Subscribe() => Server.Subscribe(Address);

    public void Unsubscribe() => Server.Unsubscribe(Address);
}
