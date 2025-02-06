using LibplanetConsole.Delegation.Grpc;

namespace LibplanetConsole.Delegation;

public readonly record struct StakeInfo(
    long ClaimableBlockIndex,
    string Deposit,
    long StartedBlockIndex,
    string GuildGold)
{
    public static implicit operator StakeInfo(StakeInfoProto stakeInfo) => new()
    {
        ClaimableBlockIndex = stakeInfo.ClaimableBlockIndex,
        Deposit = stakeInfo.Deposit,
        StartedBlockIndex = stakeInfo.StartedBlockIndex,
        GuildGold = stakeInfo.GuildGold,
    };

    public static implicit operator StakeInfoProto(StakeInfo stakeInfo) => new()
    {
        ClaimableBlockIndex = stakeInfo.ClaimableBlockIndex,
        Deposit = stakeInfo.Deposit,
        StartedBlockIndex = stakeInfo.StartedBlockIndex,
        GuildGold = stakeInfo.GuildGold,
    };
}
