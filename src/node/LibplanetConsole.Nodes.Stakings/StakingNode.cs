using System.ComponentModel.Composition;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Types.Consensus;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Nodes.Banks;
using LibplanetConsole.Nodes.Stakings.Extensions;
using LibplanetConsole.Stakings.Serializations;

namespace LibplanetConsole.Nodes.Stakings;

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
        await node.AddTransactionAsync(actions, cancellationToken);
        return new ValidatorInfo(node.GetService<BlockChain>().GetWorldState(), node.Address);
    }

    public async Task<ValidatorInfo> DelegateAsync(
        AppAddress nodeAddress, double amount, CancellationToken cancellationToken)
    {
        var validatorAddress = nodeAddress.DeriveAsValidator();
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
        return new ValidatorInfo(node.GetService<BlockChain>().GetWorldState(), node.Address);
    }

    public async Task<ValidatorInfo> UndelegateAsync(
        AppAddress nodeAddress, long shareAmount, CancellationToken cancellationToken)
    {
        var validatorAddress = nodeAddress.DeriveAsValidator();
        var actions = new IAction[]
        {
            new Nekoyume.Action.DPoS.Undelegate
            {
                Validator = validatorAddress,
                ShareAmount = Nekoyume.Action.DPoS.Misc.Asset.Share * shareAmount,
            },
        };
        await node.AddTransactionAsync(actions, cancellationToken);
        return new ValidatorInfo(node.GetService<BlockChain>().GetWorldState(), node.Address);
    }

    public async Task<ValidatorInfo> RedelegateAsync(
        AppAddress srcNodeAddress,
        AppAddress destNodeAddress,
        long shareAmount,
        CancellationToken cancellationToken)
    {
        var srcAddress = Nekoyume.Action.DPoS.Model.Validator.DeriveAddress(srcNodeAddress);
        var destAddress = Nekoyume.Action.DPoS.Model.Validator.DeriveAddress(destNodeAddress);
        var actions = new IAction[]
        {
            new Nekoyume.Action.DPoS.Redelegate
            {
                SrcValidator = srcAddress,
                DstValidator = destAddress,
                ShareAmount = Nekoyume.Action.DPoS.Misc.Asset.Share * shareAmount,
            },
        };
        await node.AddTransactionAsync(actions, cancellationToken);
        return new ValidatorInfo(node.GetService<BlockChain>().GetWorldState(), node.Address);
    }

    public async Task<ValidatorInfo[]> GetValidatorsAsync(CancellationToken cancellationToken)
    {
        return await application.InvokeAsync(Func, cancellationToken);

        ValidatorInfo[] Func()
        {
            var worldState = node.GetService<BlockChain>().GetWorldState();
            var validatorSet = worldState.GetValidatorSet();
            var validators = validatorSet.Validators;
            return [.. validators.Select(GetValidatorInfo)];

            ValidatorInfo GetValidatorInfo(Validator validator)
                => new(worldState, validator.OperatorAddress);
        }
    }

    public async Task<ValidatorInfo> GetValidatorAsync(
        AppAddress nodeAddress, CancellationToken cancellationToken)
    {
        return await application.InvokeAsync(Func, cancellationToken);

        ValidatorInfo Func()
        {
            var worldState = node.GetService<BlockChain>().GetWorldState();
            var validatorSet = worldState.GetValidatorSet();
            var validators = validatorSet.Validators;
            var validator = validators.First(item => item.OperatorAddress == (Address)nodeAddress);
            return new ValidatorInfo(worldState, validator.OperatorAddress);
        }
    }
}
