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
    bool IsActive)
{
    public static implicit operator DelegateeInfo(DelegateeInfoProto delegateeInfo) => new()
    {
        Power = delegateeInfo.Power,
        TotalShare = delegateeInfo.TotalShare,
        IsJailed = delegateeInfo.IsJailed,
        JailedUntil = delegateeInfo.JailedUntil,
        Commission = delegateeInfo.Commission,
        Tombstoned = delegateeInfo.Tombstoned,
        IsActive = delegateeInfo.IsActive,
    };

    public static implicit operator DelegateeInfoProto(DelegateeInfo delegateeInfo) => new()
    {
        Power = delegateeInfo.Power,
        TotalShare = delegateeInfo.TotalShare,
        IsJailed = delegateeInfo.IsJailed,
        JailedUntil = delegateeInfo.JailedUntil,
        Commission = delegateeInfo.Commission,
        Tombstoned = delegateeInfo.Tombstoned,
        IsActive = delegateeInfo.IsActive,
    };
}
