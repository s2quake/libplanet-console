#if LIBPLANET_NODE
using Libplanet.Action.State;
using Libplanet.Blockchain;
using LibplanetConsole.Common;
using LibplanetConsole.Node;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Delegation;

public readonly partial record struct DelegatorInfo
{
    public DelegatorInfo(INode node, Address clientAddress)
        : this(node.GetRequiredService<BlockChain>().GetWorldState(), clientAddress)
    {
    }

    private DelegatorInfo(IWorldState worldState, Address clientAddress)
    {
        // Address = clientAddress;
        // Balance = $"{worldState.GetBalance(clientAddress, worldState.GetGoldCurrency())}";
        // Delegations = GetDelegations(worldState, clientAddress);
        // Undelegations = GetUndelegations(worldState, clientAddress);
        // Redelegations = GetRedelegations(worldState, clientAddress);
    }

    // private static DelegationInfo[] GetDelegations(IWorldState worldState, Address clientAddress)
    // {
    //     var validatorSet = GetValidatorSet(worldState, ReservedAddress.BondedValidatorSet)!;
    //     var validatorAddresses = validatorSet.Set.Select(item => item.ValidatorAddress).ToArray();
    //     var delegationList = new List<DelegationInfo>();
    //     foreach (var validatorAddress in validatorAddresses)
    //     {
    //         var delegationSet = GetValidatorDelegationSet(worldState, validatorAddress)!;
    //         var delegationAddreses = delegationSet.Set;
    //         foreach (var delegationAddress in delegationAddreses)
    //         {
    //             var delegation = DelegateCtrl.GetDelegation(worldState, delegationAddress)!;
    //             if (delegation.DelegatorAddress == clientAddress)
    //             {
    //                 delegationList.Add(new DelegationInfo(worldState, delegationAddress));
    //             }
    //         }
    //     }

    //     return [.. delegationList];
    // }

    // private static UndelegationInfo[] GetUndelegations(
    //     IWorldState worldState, Address clientAddress)
    // {
    //     return UndelegateCtrl.GetUndelegationsByDelegator(worldState, clientAddress)
    //                          .Select(item => new UndelegationInfo(worldState, item.Address))
    //                          .ToArray();
    // }

    // private static RedelegationInfo[] GetRedelegations(
    //     IWorldState worldState, Address clientAddress)
    // {
    //     return RedelegateCtrl.GetRedelegationsByDelegator(worldState, clientAddress)
    //                          .Select(item => new RedelegationInfo(worldState, item.Address))
    //                          .ToArray();
    // }
}
#endif // LIBPLANET_NODE
