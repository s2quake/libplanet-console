using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Examples.Services;

namespace LibplanetConsole.Clients.Examples;

[Export]
internal sealed class ExampleRemoteNodeService
    : RemoteService<IExampleNodeService, IExampleNodeCallback>,
    IExampleNodeCallback
{
    public void OnSubscribed(AppAddress address)
    {
    }

    public void OnUnsubscribed(AppAddress address)
    {
    }
}
