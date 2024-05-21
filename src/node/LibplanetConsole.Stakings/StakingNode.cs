using System.ComponentModel.Composition;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using Libplanet.Types.Consensus;
using LibplanetConsole.Banks;
using LibplanetConsole.Nodes;
using LibplanetConsole.Stakings.Serializations;

namespace LibplanetConsole.Stakings;

[Export]
[Export(typeof(IStakingNode))]
[method: ImportingConstructor]
internal sealed class StakingNode(IApplication application, INode node) : IStakingNode
{
    public async Task<ValidatorInfo> PromoteAsync(
        double amount, CancellationToken cancellationToken)
    {
        var publicKey = node.PublicKey;
        var ncg = AssetUtility.GetNCG(amount);
        var actions = new IAction[]
        {
            new Nekoyume.Action.DPoS.PromoteValidator(publicKey, ncg),
        };
        var validatorAddress = Nekoyume.Action.DPoS.Model.Validator.DeriveAddress(node.Address);
        await node.AddTransactionAsync(actions, cancellationToken);
        return new ValidatorInfo(node.BlockChain.GetWorldState(), validatorAddress);
    }

    public async Task<ValidatorInfo> DelegateAsync(
        Address nodeAddress, double amount, CancellationToken cancellationToken)
    {
        var validatorAddress = Nekoyume.Action.DPoS.Model.Validator.DeriveAddress(nodeAddress);
        var ncg = AssetUtility.GetNCG(amount);
        var actions = new IAction[]
        {
            new Nekoyume.Action.DPoS.Delegate
            {
                Validator = validatorAddress,
                Amount = ncg,
            },
        };
        await node.AddTransactionAsync(actions, cancellationToken);
        return new ValidatorInfo(node.BlockChain.GetWorldState(), node.Address);
    }

    public async Task<ValidatorInfo> UndelegateAsync(
        Address nodeAddress, long shareAmount, CancellationToken cancellationToken)
    {
        var validatorAddress = Nekoyume.Action.DPoS.Model.Validator.DeriveAddress(nodeAddress);
        var actions = new IAction[]
        {
            new Nekoyume.Action.DPoS.Undelegate
            {
                Validator = validatorAddress,
                ShareAmount = Nekoyume.Action.DPoS.Misc.Asset.Share * shareAmount,
            },
        };
        await node.AddTransactionAsync(actions, cancellationToken);
        return new ValidatorInfo(node.BlockChain.GetWorldState(), node.Address);
    }

    public async Task<ValidatorInfo[]> GetValidatorsAsync(CancellationToken cancellationToken)
    {
        return await application.InvokeAsync(() =>
        {
            var worldState = node.BlockChain.GetWorldState();
            var validatorSet = worldState.GetValidatorSet();
            var validators = validatorSet.Validators;
            return validators.Select(GetValidatorInfo).ToArray();

            ValidatorInfo GetValidatorInfo(Validator validator)
                => new(worldState, validator.OperatorAddress);
        });
    }

    public async Task<ValidatorInfo> GetValidatorAsync(
        Address nodeAddress, CancellationToken cancellationToken)
    {
        return await application.InvokeAsync(() =>
        {
            var worldState = node.BlockChain.GetWorldState();
            var validatorSet = worldState.GetValidatorSet();
            var validators = validatorSet.Validators;
            var validator = validators.First(item => item.OperatorAddress == nodeAddress);
            return new ValidatorInfo(worldState, validator.OperatorAddress);
        });
    }
}
