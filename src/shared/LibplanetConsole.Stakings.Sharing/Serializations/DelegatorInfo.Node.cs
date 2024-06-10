#if LIBPLANET_NODE
using System.Text.Json.Serialization;
using Libplanet.Action.State;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Nodes;
using Nekoyume.Action.DPoS.Control;
using Nekoyume.Action.DPoS.Misc;
using Nekoyume.Module;
using static Nekoyume.Action.DPoS.Control.ValidatorDelegationSetCtrl;
using static Nekoyume.Action.DPoS.Control.ValidatorSetCtrl;

namespace LibplanetConsole.Stakings.Serializations;

public readonly partial record struct DelegatorInfo
{
    public DelegatorInfo(INode node, AppAddress clientAddress)
        : this(node.GetService<BlockChain>().GetWorldState(), clientAddress)
    {
    }

    private DelegatorInfo(IWorldState worldState, AppAddress clientAddress)
    {
        Address = clientAddress;
        Balance = $"{worldState.GetBalance(clientAddress, worldState.GetGoldCurrency())}";
        Delegations = GetDelegations(worldState, clientAddress);
        Undelegations = GetUndelegations(worldState, clientAddress);
        Redelegations = GetRedelegations(worldState, clientAddress);
    }

    private static DelegationInfo[] GetDelegations(IWorldState worldState, Address clientAddress)
    {
        var validatorSet = GetValidatorSet(worldState, ReservedAddress.BondedValidatorSet)!;
        var validatorAddresses = validatorSet.Set.Select(item => item.ValidatorAddress).ToArray();
        var delegationList = new List<DelegationInfo>();
        foreach (var validatorAddress in validatorAddresses)
        {
            var delegationSet = GetValidatorDelegationSet(worldState, validatorAddress)!;
            var delegationAddreses = delegationSet.Set;
            foreach (var delegationAddress in delegationAddreses)
            {
                var delegation = DelegateCtrl.GetDelegation(worldState, delegationAddress)!;
                if (delegation.DelegatorAddress == clientAddress)
                {
                    delegationList.Add(new DelegationInfo(worldState, delegationAddress));
                }
            }
        }

        return [.. delegationList];
    }

    private static UndelegationInfo[] GetUndelegations(
        IWorldState worldState, Address clientAddress)
    {
        return UndelegateCtrl.GetUndelegationsByDelegator(worldState, clientAddress)
                             .Select(item => new UndelegationInfo(worldState, item.Address))
                             .ToArray();
    }

    private static RedelegationInfo[] GetRedelegations(
        IWorldState worldState, Address clientAddress)
    {
        return RedelegateCtrl.GetRedelegationsByDelegator(worldState, clientAddress)
                             .Select(item => new RedelegationInfo(worldState, item.Address))
                             .ToArray();
    }
}
#endif // LIBPLANET_NODE
