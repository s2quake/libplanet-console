using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Example.Services;

namespace LibplanetConsole.Client.Example;

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
