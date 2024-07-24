using LibplanetConsole.Nodes;

#if LIBPLANET_CONSOLE
namespace LibplanetConsole.Consoles;
#elif LIBPLANET_NODE
namespace LibplanetConsole.Nodes;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Clients;
#else
#error "Either LIBPLANET_CONSOLE or LIBPLANET_NODE must be defined."
#endif

public sealed class BlockEventArgs(BlockInfo blockInfo) : EventArgs
{
    public BlockInfo BlockInfo { get; } = blockInfo;
}
