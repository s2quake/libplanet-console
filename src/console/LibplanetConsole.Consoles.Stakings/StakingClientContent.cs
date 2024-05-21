using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Stakings.Serializations;
using LibplanetConsole.Stakings.Services;

namespace LibplanetConsole.Consoles.Stakings;

[Export(typeof(IClientContent))]
[Export(typeof(IClientContentService))]
[Export(typeof(IStakingContent))]
[method: ImportingConstructor]
internal sealed class StakingClientContent(IClient client)
    : IClientContent, IStakingContent, IClientContentService
{
    private readonly RemoteService<IStakingService> _stakingService = new();

    IRemoteService IClientContentService.RemoteService => _stakingService;

    IClient IClientContent.Client => client;

    private IStakingService Service => _stakingService.Service;

    Task<ValidatorInfo> IStakingContent.PromoteAsync(
        double amount, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Promote is not supported.");
    }

    Task<ValidatorInfo> IStakingContent.DelegateAsync(
        Address nodeAddress, double amount, CancellationToken cancellationToken)
    {
        var signature = client.Sign(amount);
        return Service.DelegateAsync(signature, nodeAddress, amount, cancellationToken);
    }

    Task<ValidatorInfo> IStakingContent.UndelegateAsync(
        Address nodeAddress, long shareAmount, CancellationToken cancellationToken)
    {
        var signature = client.Sign(shareAmount);
        return Service.UndelegateAsync(signature, nodeAddress, shareAmount, cancellationToken);
    }

    Task<ValidatorInfo[]> IStakingContent.GetValidatorsAsync(CancellationToken cancellationToken)
        => Service.GetValidatorsAsync(cancellationToken);

    Task<ValidatorInfo> IStakingContent.GetValidatorAsync(
        Address nodeAddress, CancellationToken cancellationToken)
        => Service.GetValidatorAsync(nodeAddress, cancellationToken);
}
