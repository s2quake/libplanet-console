using LibplanetConsole.Nodes.Serializations;

namespace LibplanetConsole.Nodes.Services;

public interface INodeCallback
{
    void OnBlockAppended(BlockInfo blockInfo);
}
