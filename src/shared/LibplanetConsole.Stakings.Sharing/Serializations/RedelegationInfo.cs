using LibplanetConsole.Common;

namespace LibplanetConsole.Stakings.Serializations;

public readonly partial record struct RedelegationInfo
{
    public RedelegationInfo()
    {
    }

    public AppAddress Address { get; init; }

    public AppAddress DelegatorAddress { get; init; }

    public AppAddress ValidatorAddress { get; init; }

    public long LatestDistributeHeight { get; init; }

    public string Share { get; init; } = string.Empty;
}
