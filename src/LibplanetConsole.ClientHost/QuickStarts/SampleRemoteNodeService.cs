using System.ComponentModel.Composition;
using JSSoft.Communication;
using LibplanetConsole.Clients.Services;
using LibplanetConsole.Common.QuickStarts;

namespace LibplanetConsole.ClientHost.QuickStarts;

[Export]
[Export(typeof(IRemoteNodeServiceProvider))]
internal sealed class SampleRemoteNodeService
    : RemoteNodeService<ISampleNodeService, ISampleNodeCallbak>, IRemoteNodeServiceProvider,
    ISampleNodeCallbak
{
    IService IRemoteNodeServiceProvider.Service => this;

    public void OnSubscribed(string address)
    {
    }

    public void OnUnsubscribed(string address)
    {
    }
}
