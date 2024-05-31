using System.ComponentModel.Composition;
using Libplanet.Action;
using Libplanet.Crypto;
using LibplanetConsole.Clients.Banks;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Stakings.Serializations;
using LibplanetConsole.Stakings.Services;

namespace LibplanetConsole.Clients.Stakings.Services;

[Export(typeof(IStakingClient))]
[Export(typeof(IRemoteService))]
[method: ImportingConstructor]
internal sealed class RemoteStakingNodeService(IClient client)
    : RemoteService<IStakingService>, IStakingClient
{
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
        await client.SendTransactionAsync(actions, cancellationToken);
        return await Service.GetValidatorAsync(nodeAddress, cancellationToken);
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
        await client.SendTransactionAsync(actions, cancellationToken);
        return await Service.GetValidatorAsync(nodeAddress, cancellationToken);
    }

    public async Task<ValidatorInfo> RedelegateAsync(
        Address srcNodeAddress,
        Address destNodeAddress,
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
        await client.SendTransactionAsync(actions, cancellationToken);
        return await Service.GetValidatorAsync(destAddress, cancellationToken);
    }

    public Task<ValidatorInfo[]> GetValidatorsAsync(CancellationToken cancellationToken)
        => Service.GetValidatorsAsync(cancellationToken);

    public Task<ValidatorInfo> GetValidatorAsync(
        Address nodeAddress, CancellationToken cancellationToken)
        => Service.GetValidatorAsync(nodeAddress, cancellationToken);
}
