using System.Text.Json.Serialization;
using LibplanetConsole.Common;

namespace LibplanetConsole.Stakings.Serializations;

public readonly partial record struct DelegationInfo
{
    public DelegationInfo()
    {
    }

    [JsonIgnore]
    public AppAddress Address { get; init; }

    public AppAddress DelegatorAddress { get; init; }

    public AppAddress ValidatorAddress { get; init; }

    public long LatestDistributeHeight { get; init; }

    public string Share { get; init; } = string.Empty;
}
