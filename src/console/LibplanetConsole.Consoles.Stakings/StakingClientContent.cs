using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Common.Services;
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
        PromoteOptions promoteOptions, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Promote is not supported.");
    }

    Task<ValidatorInfo> IStakingContent.DelegateAsync(
        DelegateOptions delegateOptions, CancellationToken cancellationToken)
    {
        return Service.DelegateAsync(delegateOptions.Sign(client), cancellationToken);
    }

    Task<ValidatorInfo> IStakingContent.UndelegateAsync(
        UndelegateOptions undelegateOptions, CancellationToken cancellationToken)
    {
        return Service.UndelegateAsync(undelegateOptions.Sign(client), cancellationToken);
    }

    Task<ValidatorInfo> IStakingContent.RedelegateAsync(
        RedelegateOptions redelegateOptions,
        CancellationToken cancellationToken)
    {
        return Service.RedelegateAsync(redelegateOptions.Sign(client), cancellationToken);
    }

    Task<ValidatorInfo[]> IStakingContent.GetValidatorsAsync(CancellationToken cancellationToken)
        => Service.GetValidatorsAsync(cancellationToken);

    Task<ValidatorInfo> IStakingContent.GetValidatorAsync(
        Address nodeAddress, CancellationToken cancellationToken)
        => Service.GetValidatorAsync(nodeAddress, cancellationToken);
}
