using System.ComponentModel.Composition;
using LibplanetConsole.Common;
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

    string IClientContent.Name => "staking";

    private IStakingService Service => _stakingService.Service;

    Task<ValidatorInfo> IStakingContent.PromoteAsync(
        PromoteOptions promoteOptions, CancellationToken cancellationToken)
        => throw new NotSupportedException("Promote is not supported.");

    Task<ValidatorInfo> IStakingContent.DelegateAsync(
        DelegateOptions delegateOptions, CancellationToken cancellationToken)
        => Service.DelegateAsync(delegateOptions.Sign(client), cancellationToken);

    Task<ValidatorInfo> IStakingContent.UndelegateAsync(
        UndelegateOptions undelegateOptions, CancellationToken cancellationToken)
        => Service.UndelegateAsync(undelegateOptions.Sign(client), cancellationToken);

    Task<ValidatorInfo> IStakingContent.RedelegateAsync(
        RedelegateOptions redelegateOptions, CancellationToken cancellationToken)
        => Service.RedelegateAsync(redelegateOptions.Sign(client), cancellationToken);

    Task<ValidatorInfo[]> IStakingContent.GetValidatorsAsync(CancellationToken cancellationToken)
        => Service.GetValidatorsAsync(cancellationToken);

    Task<ValidatorInfo> IStakingContent.GetValidatorAsync(
        AppAddress nodeAddress, CancellationToken cancellationToken)
        => Service.GetValidatorAsync(nodeAddress, cancellationToken);
}
