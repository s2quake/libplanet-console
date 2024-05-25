using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Examples;

[Export(typeof(ILocalService))]
[method: ImportingConstructor]
internal sealed class ExampleClientService(IExampleClientContent sampleClient)
    : LocalService<IExampleClientService>, IExampleClientService
{
    public void Subscribe()
    {
        sampleClient.Subscribe();
    }

    public void Unsubscribe()
    {
        sampleClient.Unsubscribe();
    }
}
