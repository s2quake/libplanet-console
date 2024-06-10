using System.Text.Json.Serialization;
using LibplanetConsole.Common;

namespace LibplanetConsole.Stakings.Serializations;

public readonly partial record struct UndelegationInfo
{
    public UndelegationInfo()
    {
    }

    [JsonIgnore]
    public AppAddress Address { get; init; }

    public AppAddress DelegatorAddress { get; init; }

    public AppAddress ValidatorAddress { get; init; }

    public UndelegationEntryInfo[] Entires { get; init; } = [];
}
