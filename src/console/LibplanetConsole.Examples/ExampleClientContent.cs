using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles;

namespace LibplanetConsole.Examples;

[Export(typeof(IExampleClientContent))]
[Export(typeof(IClientContent))]
[method: ImportingConstructor]
internal sealed class ExampleClientContent(IClient client)
    : ClientContentBase(client), IExampleClientContent
{
    private readonly RemoteService<IExampleClientService> _remoteService = new();

    private IExampleClientService Service => _remoteService.Service;

    public void Subscribe() => Service.Subscribe();

    public void Unsubscribe() => Service.Unsubscribe();
}
