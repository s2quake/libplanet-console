using System.ComponentModel.Composition;
using LibplanetConsole.Examples;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Examples;

[Export]
internal sealed class ExampleRemoteNodeService
    : RemoteService<IExampleNodeService, IExampleNodeCallbak>,
    IExampleNodeCallbak
{
    public void OnSubscribed(string address)
    {
    }

    public void OnUnsubscribed(string address)
    {
    }
}
