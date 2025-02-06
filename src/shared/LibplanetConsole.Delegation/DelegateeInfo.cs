using LibplanetConsole.Delegation.Grpc;

namespace LibplanetConsole.Delegation;

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
