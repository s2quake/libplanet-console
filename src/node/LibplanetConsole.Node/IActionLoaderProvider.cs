using Libplanet.Action.Loader;

namespace LibplanetConsole.Nodes;

public interface IActionLoaderProvider
{
    IActionLoader GetActionLoader();
}
