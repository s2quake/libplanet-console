using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Examples;

[Export(typeof(IExampleClientContent))]
[Export(typeof(IClientContent))]
[method: ImportingConstructor]
internal sealed class ExampleClientContent(IClient client)
    : ClientContentBase(client), IExampleClientContent
{
    private readonly RemoteService<IExampleClientService> _remoteService = new();

    public bool IsExample { get; }
        = ApplicationSettingsParser.Peek<ExampleClientSettings>().IsClientExample;

    private IExampleClientService Service => _remoteService.Service;

    public void Subscribe() => Service.Subscribe();

    public void Unsubscribe() => Service.Unsubscribe();
}
