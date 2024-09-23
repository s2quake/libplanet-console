using Libplanet.Action.Loader;

namespace LibplanetConsole.Node;

public interface IActionLoaderProvider
{
    IActionLoader GetActionLoader();
}
