namespace LibplanetConsole.Nodes.Services;

public interface IBlockChainCallback
{
    void OnBlockAppended(BlockInfo blockInfo);
}
