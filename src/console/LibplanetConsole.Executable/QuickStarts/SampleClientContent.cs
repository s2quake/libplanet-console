using System.ComponentModel.Composition;
using JSSoft.Communication;
using LibplanetConsole.Common.QuickStarts;

namespace LibplanetConsole.Executable.QuickStarts;

[Export(typeof(ISampleClient))]
[Export(typeof(IClientContent))]
[method: ImportingConstructor]
internal sealed class SampleClientContent(IClient client)
    : ClientContentBase(client), IRemoteServiceProvider, ISampleClient
{
    private readonly RemoteService<ISampleClientService> _service = new();

    private ISampleClientService Server => _service.Server;

    public void Subscribe() => Server.Subscribe();

    public void Unsubscribe() => Server.Unsubscribe();

    IService IRemoteServiceProvider.GetService(object obj) => _service;
}
