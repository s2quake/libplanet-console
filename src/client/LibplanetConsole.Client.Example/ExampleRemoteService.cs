using LibplanetConsole.Common.Services;
using LibplanetConsole.Example.Services;

namespace LibplanetConsole.Client.Example;

internal sealed class ExampleRemoteService
    : RemoteService<IExampleNodeService, IExampleNodeCallback>, IExampleNodeCallback
{
    public void OnSubscribed(Address address)
    {
    }

    public void OnUnsubscribed(Address address)
    {
    }
}
