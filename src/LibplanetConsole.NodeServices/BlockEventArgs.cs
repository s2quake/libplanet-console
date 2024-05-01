using LibplanetConsole.NodeServices.Serializations;

namespace LibplanetConsole.NodeServices;

public sealed class BlockEventArgs(BlockInfo blockInfo) : EventArgs
{
    public BlockInfo BlockInfo { get; } = blockInfo;
}
