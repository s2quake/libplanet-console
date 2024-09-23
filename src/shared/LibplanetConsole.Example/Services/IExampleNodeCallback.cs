using LibplanetConsole.Common;

namespace LibplanetConsole.Example.Services;

public interface IExampleNodeCallback
{
    void OnSubscribed(AppAddress address);

    void OnUnsubscribed(AppAddress address);
}
