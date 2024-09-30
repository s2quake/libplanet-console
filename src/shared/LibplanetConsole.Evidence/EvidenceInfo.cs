using Libplanet.Types.Evidence;

namespace LibplanetConsole.Evidence;

public readonly record struct EvidenceInfo
{
    public string Type { get; init; }

    public string Id { get; init; }

    public Address TargetAddress { get; init; }

    public long Height { get; init; }

    public DateTimeOffset Timestamp { get; init; }

    public static explicit operator EvidenceInfo(EvidenceBase evidence)
    {
        return new EvidenceInfo
        {
            Type = evidence.GetType().Name,
            Id = evidence.Id.ToString(),
            Height = evidence.Height,
            TargetAddress = evidence.TargetAddress,
            Timestamp = evidence.Timestamp,
        };
    }
}
