using LibplanetConsole.Example.Services;

namespace LibplanetConsole.Client.Example;

internal sealed class Example(
    IClient client,
    ExampleRemoteService remoteNodeService,
    ExampleSettings settings)
    : IExample
{
    private readonly ExampleRemoteService _remoteNodeService = remoteNodeService;

    public Address Address => client.Address;

    public bool IsExample { get; } = settings.IsExample;

    private IExampleNodeService Server => _remoteNodeService.Service;

    public void Subscribe() => Server.Subscribe(Address);

    public void Unsubscribe() => Server.Unsubscribe(Address);
}
