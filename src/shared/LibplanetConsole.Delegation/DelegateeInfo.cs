namespace LibplanetConsole.Delegation;

public readonly partial record struct DelegateeInfo
{
    public DelegateeInfo()
    {
    }

    public Address Address { get; init; }

    public string TotalDelegated { get; init; } = string.Empty;

    public string TotalShares { get; init; } = string.Empty;

    public bool Jailed { get; init; }

    public long JailedUntil { get; init; } = -1;

    public bool Tombstoned { get; init; }

    public string RewardPool { get; init; } = string.Empty;
}
