namespace LibplanetConsole.Blockchain;

public sealed class BlockEventArgs(BlockInfo blockInfo) : EventArgs
{
    public BlockInfo BlockInfo { get; } = blockInfo;
}
