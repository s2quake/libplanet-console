using LibplanetConsole.Common.Services;
using LibplanetConsole.Example.Services;

namespace LibplanetConsole.Console.Example;

internal sealed class ExampleClient(IClient client, ExampleClientSettings settings)
    : ClientContentBase(client), IExampleClient
{
    private readonly RemoteService<IExampleClientService> _remoteService = new();

    public bool IsExample { get; } = settings.IsClientExample;

    private IExampleClientService Service => _remoteService.Service;

    public void Subscribe() => Service.Subscribe();

    public void Unsubscribe() => Service.Unsubscribe();
}
