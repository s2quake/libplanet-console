using System.ComponentModel.Composition;
using LibplanetConsole.Common.QuickStarts;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.ClientHost.QuickStarts;

[Export(typeof(ILocalService))]
[method: ImportingConstructor]
internal sealed class SampleClientService(ISampleClient sampleClient)
    : LocalService<ISampleClientService>, ISampleClientService
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
