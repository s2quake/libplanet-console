// using System.ComponentModel.Composition;
// using JSSoft.Library.Commands;
// using OnBoarding.ConsoleHost.Extensions;
// using OnBoarding.ConsoleHost.Serializations;

// namespace OnBoarding.ConsoleHost.Commands;

// [Export(typeof(ICommand))]
// [CommandSummary("Display block commands.")]
// sealed class BlockCommand : CommandMethodBase
// {
//     private readonly Application _application;

//     [ImportingConstructor]
//     public BlockCommand(Application application)
//     {
//         _application = application;
//     }

//     [CommandMethod]
//     [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
//     public void List()
//     {
//         var blockChain = _application.GetBlockChain(IndexProperties.SwarmIndex);
//         var blockChainInfo = new BlockChainInfo(blockChain);
//         Out.WriteLineAsJson(blockChainInfo);
//     }

//     [CommandMethod]
//     [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
//     public void Info(int blockIndex = -1)
//     {
//         var blockChain = _application.GetBlockChain(IndexProperties.SwarmIndex);
//         var block = blockChain[blockIndex];
//         var blockInfo = new BlockInfo(block);
//         Out.WriteLineAsJson(blockInfo);
//     }
// }
