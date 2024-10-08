using LibplanetConsole.Node;

namespace LibplanetConsole.Node;

public sealed class BlockEventArgs(BlockInfo blockInfo) : EventArgs
{
    public BlockInfo BlockInfo { get; } = blockInfo;
}
