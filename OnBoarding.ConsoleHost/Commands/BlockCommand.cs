using System.ComponentModel.Composition;
using JSSoft.Library.Commands;
using OnBoarding.ConsoleHost.Serializations;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
[CommandSummary("Display block commands.")]
sealed class BlockCommand(Application application) : CommandMethodBase
{
    private readonly Application _application = application;

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(SwarmProperties), nameof(SwarmProperties.Index))]
    public void List()
    {
        var blockChain = _application.GetBlockChain(SwarmProperties.Index);
        var blockChainInfo = new BlockChainInfo(blockChain);
        var json = JsonUtility.SerializeObject(blockChainInfo, isColorized: true);
        Out.Write(json);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(SwarmProperties), nameof(SwarmProperties.Index))]
    public void Info(int blockIndex = -1)
    {
        var blockChain = _application.GetBlockChain(SwarmProperties.Index);
        var block = blockChain[blockIndex];
        var blockInfo = new BlockInfo(block);
        var json = JsonUtility.SerializeObject(blockInfo, isColorized: true);
        Out.Write(json);
    }
}
