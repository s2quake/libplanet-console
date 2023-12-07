using System.Text;
using JSSoft.Library.Terminals;
using Libplanet.Blockchain;

namespace OnBoarding.ConsoleHost.Extensions;

static class StringBuilderExtensions
{
    public static void AppendStatesLine(this StringBuilder @this, BlockChain blockChain, UserCollection users)
    {
        for (var i = 0L; i < blockChain.Count; i++)
        {
            AppendStatesLine(@this, blockChain, index: i, users);
        }
    }

    public static void AppendStatesLine(this StringBuilder @this, BlockChain blockChain, long index, UserCollection users)
    {
        var tsb = new TerminalStringBuilder();
        var block = blockChain[index];
        tsb.IsBold = true;
        tsb.AppendLine($"Block index #{index}: {block.Hash}");
        tsb.IsBold = false;
        tsb.AppendLine($"    Transactions: {block.Transactions.Count}");
        tsb.AppendLine();

        @this.Append(tsb.ToString());
    }
}
