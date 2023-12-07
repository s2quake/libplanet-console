using System.ComponentModel.Composition;
using JSSoft.Library.Commands;
using OnBoarding.ConsoleHost.Extensions;
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
        Out.WriteLineAsJson(blockChainInfo);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(SwarmProperties), nameof(SwarmProperties.Index))]
    public void Info(int blockIndex = -1)
    {
        var blockChain = _application.GetBlockChain(SwarmProperties.Index);
        var block = blockChain[blockIndex];
        var blockInfo = new BlockInfo(block);
        Out.WriteLineAsJson(blockInfo);
    }
}
