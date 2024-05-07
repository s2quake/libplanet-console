using LibplanetConsole.Nodes.Serializations;

namespace LibplanetConsole.Nodes;

public sealed class BlockEventArgs(BlockInfo blockInfo) : EventArgs
{
    public BlockInfo BlockInfo { get; } = blockInfo;
}
