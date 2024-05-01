using Libplanet.Action.Loader;
using Libplanet.Crypto;
using LibplanetConsole.NodeServices;

namespace LibplanetConsole.NodeHost;

internal sealed class Node(ApplicationOptions options, IEnumerable<IActionLoader> actionLoaders)
    : NodeBase(GetPrivateKey(options)), INode
{
    public override IActionLoader[] ActionLoaders { get; } = [.. actionLoaders];

    public static PrivateKey GetPrivateKey(ApplicationOptions options)
    {
        if (options.PrivateKey == string.Empty)
        {
            return new PrivateKey();
        }

        return new PrivateKey(options.PrivateKey);
    }
}
