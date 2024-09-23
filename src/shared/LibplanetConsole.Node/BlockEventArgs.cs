using LibplanetConsole.Node;

#if LIBPLANET_CONSOLE
namespace LibplanetConsole.Console;
#elif LIBPLANET_NODE
namespace LibplanetConsole.Node;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client;
#else
#error "Either LIBPLANET_CONSOLE or LIBPLANET_NODE must be defined."
#endif

public sealed class BlockEventArgs(BlockInfo blockInfo) : EventArgs
{
    public BlockInfo BlockInfo { get; } = blockInfo;
}
