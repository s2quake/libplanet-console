namespace LibplanetConsole.Node.Services;

public interface IBlockChainCallback
{
    void OnBlockAppended(BlockInfo blockInfo);
}
