using Libplanet.Action.State;

namespace LibplanetConsole.Bank;

public record class PoolInfo
{
    public PoolInfo(IWorldState worldState)
    {
        // Bonded = GetBalance(worldState, ReservedAddress.BondedPool);
        // Unbonded = GetBalance(worldState, ReservedAddress.UnbondedPool);
        // Reward = GetBalance(worldState, ReservedAddress.RewardPool);
        // Community = GetBalance(worldState, ReservedAddress.CommunityPool);
    }

    public PoolInfo()
    {
    }

    public string Bonded { get; init; } = string.Empty;

    public string Unbonded { get; init; } = string.Empty;

    public string Reward { get; init; } = string.Empty;

    public string Community { get; init; } = string.Empty;

    // private static string GetBalance(IWorldState worldState, Address address)
    // {
    //     return $"{worldState.GetBalance(address, worldState.GetGoldCurrency())}";
    // }
}
