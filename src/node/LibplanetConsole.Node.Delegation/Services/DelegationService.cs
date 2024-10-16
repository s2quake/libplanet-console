// using LibplanetConsole.Common.Services;
// using LibplanetConsole.Delegation;
// using LibplanetConsole.Delegation.Services;

// namespace LibplanetConsole.Node.Delegation.Services;

// internal sealed class DelegationService(Delegation delegationNode, INode node)
//     : LocalService<IDelegationService>, IDelegationService
// {
//     public Task<DelegateeInfo> PromoteAsync(
//         PromoteOptions promoteOptions, CancellationToken cancellationToken)
//     {
//         promoteOptions.Verify(node);
//         return delegationNode.PromoteAsync(promoteOptions.Amount, cancellationToken);
//     }

//     public Task<BondInfo> DelegateAsync(
//         DelegateOptions delegateOptions, CancellationToken cancellationToken)
//     {
//         delegateOptions.Verify(node);
//         return delegationNode.DelegateAsync(
//             nodeAddress: delegateOptions.NodeAddress,
//             amount: delegateOptions.Amount,
//             cancellationToken: cancellationToken);
//     }

//     public Task<DelegateeInfo> UndelegateAsync(
//         UndelegateOptions undelegateOptions,
//         CancellationToken cancellationToken)
//     {
//         undelegateOptions.Verify(node);
//         return delegationNode.UndelegateAsync(
//             nodeAddress: undelegateOptions.NodeAddress,
//             shareAmount: undelegateOptions.ShareAmount,
//             cancellationToken: cancellationToken);
//     }

//     public Task<DelegateeInfo> RedelegateAsync(
//         RedelegateOptions redelegateOptions,
//         CancellationToken cancellationToken)
//     {
//         redelegateOptions.Verify(node);
//         return delegationNode.RedelegateAsync(
//             srcNodeAddress: redelegateOptions.SrcNodeAddress,
//             destNodeAddress: redelegateOptions.DestNodeAddress,
//             shareAmount: redelegateOptions.ShareAmount,
//             cancellationToken: cancellationToken);
//     }

//     public Task<DelegateeInfo[]> GetValidatorsAsync(CancellationToken cancellationToken)
//         => delegationNode.GetValidatorsAsync(cancellationToken);

//     public Task<DelegateeInfo> GetValidatorAsync(
//         Address nodeAddress, CancellationToken cancellationToken)
//         => delegationNode.GetValidatorAsync(nodeAddress, cancellationToken);
// }
