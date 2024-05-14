using System.ComponentModel.Composition;
using LibplanetConsole.Common.QuickStarts;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles;

namespace LibplanetConsole.ConsoleHost.QuickStarts;

[Export(typeof(ISampleClientContent))]
[Export(typeof(IClientContent))]
[method: ImportingConstructor]
internal sealed class SampleClientContent(IClient client)
    : ClientContentBase(client), ISampleClientContent
{
    private readonly RemoteService<ISampleClientService> _remoteService = new();

    private ISampleClientService Service => _remoteService.Service;

    public void Subscribe() => Service.Subscribe();

    public void Unsubscribe() => Service.Unsubscribe();
}
