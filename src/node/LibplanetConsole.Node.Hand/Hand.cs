using Libplanet.Action.State;
using LibplanetConsole.BlockChain;

namespace LibplanetConsole.Node.Hand;

internal sealed class Hand(INode node, IBlockChain blockChain)
    : NodeContentBase(nameof(Hand)), IHand
{

}
