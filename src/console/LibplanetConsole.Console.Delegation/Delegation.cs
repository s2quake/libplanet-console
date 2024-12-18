using Microsoft.Extensions.DependencyInjection;
using Nekoyume.Action;

namespace LibplanetConsole.Console.Delegation;

internal sealed class Delegation(IConsole console, RunningNode runningNode)
    : ConsoleContentBase("delegation"), IDelegation
{
    public async Task StakeAsync(long ncg, CancellationToken cancellationToken)
    {
        var stakeAction = new Stake(ncg);
        await console.SendTransactionAsync([stakeAction], cancellationToken);
    }

    public Task<DelegateeInfo> GetDelegateeInfoAsync(
        Address address, CancellationToken cancellationToken)
    {
        var nodeDelegation = runningNode.Node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        return nodeDelegation.GetDelegateeInfoAsync(address, cancellationToken);
    }

    public Task<DelegatorInfo> GetDelegatorInfoAsync(
        Address address, CancellationToken cancellationToken)
    {
        var nodeDelegation = runningNode.Node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        return nodeDelegation.GetDelegatorInfoAsync(address, cancellationToken);
    }

    public Task<StakeInfo> GetStakeInfoAsync(
        Address address, CancellationToken cancellationToken)
    {
        var nodeDelegation = runningNode.Node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        return nodeDelegation.GetStakeInfoAsync(address, cancellationToken);
    }
}
