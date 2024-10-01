using System.Text.Json.Serialization;

namespace LibplanetConsole.Delegation;

public readonly partial record struct DelegatorInfo
{
    public DelegatorInfo()
    {
    }

    [JsonIgnore]
    public Address Address { get; init; }

    public string Balance { get; init; } = string.Empty;

    public DelegationInfo[] Delegations { get; init; } = [];

    public UndelegationInfo[] Undelegations { get; init; } = [];

    public RedelegationInfo[] Redelegations { get; init; } = [];
}
