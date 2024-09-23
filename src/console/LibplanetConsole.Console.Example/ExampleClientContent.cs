using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Example.Services;

namespace LibplanetConsole.Console.Example;

[Export(typeof(IExampleClientContent))]
[Export(typeof(IClientContent))]
[method: ImportingConstructor]
internal sealed class ExampleClientContent(IClient client, ExampleClientSettings settings)
    : ClientContentBase(client), IExampleClientContent
{
    private readonly RemoteService<IExampleClientService> _remoteService = new();

    public bool IsExample { get; } = settings.IsClientExample;

    private IExampleClientService Service => _remoteService.Service;

    public void Subscribe() => Service.Subscribe();

    public void Unsubscribe() => Service.Unsubscribe();
}
