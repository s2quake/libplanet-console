namespace LibplanetConsole.Delegation;

public readonly partial record struct RedelegationInfo
{
    public RedelegationInfo()
    {
    }

    public Address Address { get; init; }

    public Address DelegatorAddress { get; init; }

    public Address ValidatorAddress { get; init; }

    public long LatestDistributeHeight { get; init; }

    public string Share { get; init; } = string.Empty;
}
