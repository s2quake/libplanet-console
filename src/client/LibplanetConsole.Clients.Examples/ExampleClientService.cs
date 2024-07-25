using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Examples.Services;

namespace LibplanetConsole.Clients.Examples;

[Export(typeof(ILocalService))]
[method: ImportingConstructor]
internal sealed class ExampleClientService(IExampleClient sampleClient)
    : LocalService<IExampleClientService>, IExampleClientService
{
    public void Subscribe() => sampleClient.Subscribe();

    public void Unsubscribe() => sampleClient.Unsubscribe();
}
