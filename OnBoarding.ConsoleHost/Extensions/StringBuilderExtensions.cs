using System.Text;
using Bencodex.Types;
using JSSoft.Library.Terminals;
using Libplanet.Action.State;
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
        var worldState = blockChain.GetWorldState(block.Hash);
        var account = worldState.GetAccount(ReservedAddresses.LegacyAccount);

        tsb.IsBold = true;
        tsb.AppendLine($"Block index #{index}: {block.Hash}");
        tsb.IsBold = false;
        tsb.AppendLine();
        tsb.AppendLine("States");
        tsb.IsBold = true;
        tsb.AppendLine("------");
        tsb.IsBold = false;
        foreach (var item in users)
        {
            var state = account.GetState(item.Address) is Integer i ? (int)i : 0;
            tsb.AppendLine($"{item}: {state}");
        }
        @this.Append(tsb.ToString());
    }
}
