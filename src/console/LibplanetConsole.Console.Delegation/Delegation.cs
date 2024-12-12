using Nekoyume.Action;

namespace LibplanetConsole.Console.Delegation;

internal sealed class Delegation(IConsole console)
    : ConsoleContentBase("delegation"), IDelegation
{
    public Task<StakeInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task StakeAsync(long ncg, CancellationToken cancellationToken)
    {
        var stakeAction = new Stake(ncg);
        await console.SendTransactionAsync([stakeAction], cancellationToken);
    }

    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
