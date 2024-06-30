using LibplanetConsole.Common;

namespace LibplanetConsole.Examples.Services;

public interface IExampleNodeCallback
{
    void OnSubscribed(AppAddress address);

    void OnUnsubscribed(AppAddress address);
}
