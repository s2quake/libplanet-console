using JSSoft.Communication;
using LibplanetConsole.NodeServices.Serializations;

namespace LibplanetConsole.NodeServices;

public interface INodeCallback
{
    [ClientMethod]
    void OnBlockAppended(BlockInfo blockInfo);
}
