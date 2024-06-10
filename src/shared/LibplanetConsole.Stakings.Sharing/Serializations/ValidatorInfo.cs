using LibplanetConsole.Common;

namespace LibplanetConsole.Stakings.Serializations;

public readonly partial record struct ValidatorInfo
{
    public ValidatorInfo()
    {
    }

    public AppAddress Address { get; init; }

    public AppAddress OperatorAddress { get; init; }

    public bool Jailed { get; init; }

    public string Status { get; init; } = string.Empty;

    public string DelegatorShares { get; init; } = string.Empty;

    public string Power { get; init; } = string.Empty;

    public string Balance { get; init; } = string.Empty;

    public DelegationInfo[] Delegations { get; init; } = [];

    public UndelegationInfo[] Undelegations { get; init; } = [];
}
