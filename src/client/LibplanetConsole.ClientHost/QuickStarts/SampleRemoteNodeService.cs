using System.ComponentModel.Composition;
using LibplanetConsole.Common.QuickStarts;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.ClientHost.QuickStarts;

[Export]
internal sealed class SampleRemoteNodeService
    : RemoteService<ISampleNodeService, ISampleNodeCallbak>,
    ISampleNodeCallbak
{
    public void OnSubscribed(string address)
    {
    }

    public void OnUnsubscribed(string address)
    {
    }
}
