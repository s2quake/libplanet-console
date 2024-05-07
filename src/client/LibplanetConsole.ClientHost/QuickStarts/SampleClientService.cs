using System.ComponentModel.Composition;
using JSSoft.Communication;
using LibplanetConsole.Common.QuickStarts;

namespace LibplanetConsole.ClientHost.QuickStarts;

[Export(typeof(IService))]
[method: ImportingConstructor]
internal sealed class SampleClientService(ISampleClient sampleClient)
    : ServerService<ISampleClientService>, ISampleClientService
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
