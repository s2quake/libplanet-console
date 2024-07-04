using Libplanet.Types.Evidence;
using LibplanetConsole.Common;

namespace LibplanetConsole.Evidence.Serializations;

public readonly record struct EvidenceInfo
{
    public string Type { get; init; }

    public string Id { get; init; }

    public AppAddress TargetAddress { get; init; }

    public long Height { get; init; }

    public DateTimeOffset Timestamp { get; init; }

    public static explicit operator EvidenceInfo(EvidenceBase evidence)
    {
        return new EvidenceInfo
        {
            Type = evidence.GetType().Name,
            Id = evidence.Id.ToString(),
            Height = evidence.Height,
            TargetAddress = (AppAddress)evidence.TargetAddress,
            Timestamp = evidence.Timestamp,
        };
    }
}
