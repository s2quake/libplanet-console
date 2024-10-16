using System.Text.Json.Serialization;

namespace LibplanetConsole.Delegation;

public readonly partial record struct UndelegationInfo
{
    public UndelegationInfo()
    {
    }

    [JsonIgnore]
    public Address Address { get; init; }

    public Address DelegatorAddress { get; init; }

    public Address ValidatorAddress { get; init; }

    public UndelegationEntryInfo[] Entires { get; init; } = [];
}
