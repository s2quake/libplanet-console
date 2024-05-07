using JSSoft.Communication;
using LibplanetConsole.Nodes.Serializations;

namespace LibplanetConsole.Nodes.Services;

public interface INodeCallback
{
    [ClientMethod]
    void OnBlockAppended(BlockInfo blockInfo);
}
