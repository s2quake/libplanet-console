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

public readonly record struct DelegateeInfo(
    string Power,
    string TotalShare,
    bool IsJailed,
    long JailedUntil,
    long Commission,
    bool Tombstoned,
    bool IsActive,
    StakeInfo StakeInfo)
{
    public static implicit operator DelegateeInfo(DelegateeInfoProto delegationInfo)
    {
        return new DelegateeInfo
        {
            Power = delegationInfo.Power,
            TotalShare = delegationInfo.TotalShare,
            IsJailed = delegationInfo.IsJailed,
            JailedUntil = delegationInfo.JailedUntil,
            Commission = delegationInfo.Commission,
            Tombstoned = delegationInfo.Tombstoned,
            IsActive = delegationInfo.IsActive,
            StakeInfo = delegationInfo.StakeInfo,
        };
    }

    public static implicit operator DelegateeInfoProto(DelegateeInfo delegationInfo)
    {
        return new DelegateeInfoProto
        {
            Power = delegationInfo.Power,
            TotalShare = delegationInfo.TotalShare,
            IsJailed = delegationInfo.IsJailed,
            JailedUntil = delegationInfo.JailedUntil,
            Commission = delegationInfo.Commission,
            Tombstoned = delegationInfo.Tombstoned,
            IsActive = delegationInfo.IsActive,
            StakeInfo = delegationInfo.StakeInfo,
        };
    }
}
