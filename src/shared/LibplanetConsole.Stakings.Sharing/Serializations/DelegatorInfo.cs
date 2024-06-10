using System.Text.Json.Serialization;
using LibplanetConsole.Common;

namespace LibplanetConsole.Stakings.Serializations;

public readonly partial record struct DelegatorInfo
{
    public DelegatorInfo()
    {
    }

    [JsonIgnore]
    public AppAddress Address { get; init; }

    public string Balance { get; init; } = string.Empty;

    public DelegationInfo[] Delegations { get; init; } = [];

    public UndelegationInfo[] Undelegations { get; init; } = [];

    public RedelegationInfo[] Redelegations { get; init; } = [];
}
