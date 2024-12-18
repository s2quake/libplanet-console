using LibplanetConsole.Grpc.Delegation;

#if LIBPLANET_NODE
namespace LibplanetConsole.Node.Delegation;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client.Delegation;
#elif LIBPLANET_CONSOLE
namespace LibplanetConsole.Console.Delegation;
#else
#error LIBPLANET_NODE, LIBPLANET_CLIENT, or LIBPLANET_CONSOLE must be defined.
#endif

public readonly record struct StakeInfo(
    long ClaimableBlockIndex,
    string Deposit,
    long StartedBlockIndex,
    long CancellableBlockIndex,
    string GuildGold)
{
    public static implicit operator StakeInfo(StakeInfoProto stakeInfo) => new()
    {
        ClaimableBlockIndex = stakeInfo.ClaimableBlockIndex,
        Deposit = stakeInfo.Deposit,
        StartedBlockIndex = stakeInfo.StartedBlockIndex,
        CancellableBlockIndex = stakeInfo.CancellableBlockIndex,
        GuildGold = stakeInfo.GuildGold,
    };

    public static implicit operator StakeInfoProto(StakeInfo stakeInfo) => new()
    {
        ClaimableBlockIndex = stakeInfo.ClaimableBlockIndex,
        Deposit = stakeInfo.Deposit,
        StartedBlockIndex = stakeInfo.StartedBlockIndex,
        CancellableBlockIndex = stakeInfo.CancellableBlockIndex,
        GuildGold = stakeInfo.GuildGold,
    };
}
