using System.ComponentModel.Composition;
using System.Text;
using JSSoft.Library.Commands;
using Libplanet.Action;
using Libplanet.Action.Loader;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Crypto;
using Libplanet.Store;
using Libplanet.Types.Blocks;
using Libplanet.Types.Consensus;
using Libplanet.Types.Tx;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
sealed class BlockCommand(BlockChain blockChain) : CommandMethodBase
{
    private readonly BlockChain _blockChain = blockChain;

    [CommandMethod]
    public void New()
    {
        Out.WriteLine(_blockChain.Count);
    }

    [CommandMethod]
    public void List()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < _blockChain.Count; i++)
        {
            var block = _blockChain[i];
            sb.Append($"[{i}]: {block.StateRootHash}");
        }
        Out.Write(sb.ToString());
    }
}
