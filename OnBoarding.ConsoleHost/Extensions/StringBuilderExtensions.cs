using System.Text;
using Bencodex.Types;
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
        var block = blockChain[index];
        var worldState = blockChain.GetWorldState(block.Hash);
        var account = worldState.GetAccount(ReservedAddresses.LegacyAccount);

        @this.AppendLine($"Block index #{index}: {block.Hash}");
        @this.AppendLine();
        @this.AppendLine("States");
        @this.AppendLine("------");
        foreach (var item in users)
        {
            var state = account.GetState(item.Address) is Integer i ? (int)i : 0;
            @this.AppendLine($"{item}: {state}");
        }
        @this.AppendLine();
    }
}
