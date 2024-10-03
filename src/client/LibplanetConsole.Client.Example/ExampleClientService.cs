using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Example.Services;

namespace LibplanetConsole.Client.Example;

[Export(typeof(ILocalService))]
internal sealed class ExampleClientService(IExampleClient sampleClient)
    : LocalService<IExampleClientService>, IExampleClientService
{
    public void Subscribe() => sampleClient.Subscribe();

    public void Unsubscribe() => sampleClient.Unsubscribe();
}
