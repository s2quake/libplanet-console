using LibplanetConsole.Common;

namespace LibplanetConsole.Evidences.Serializations;

public record class EvidenceInfo
{
    public string Type { get; init; } = string.Empty;

    public string Id { get; init; } = default!;

    public string TargetAddress { get; init; } = string.Empty;

    public long Height { get; init; }

    public DateTimeOffset Timestamp { get; init; }

    public static explicit operator EvidenceInfo(Libplanet.Types.Evidences.Evidence evidence)
    {
        return new EvidenceInfo
        {
            Type = evidence.GetType().Name,
            Id = evidence.Id.ToString(),
            Height = evidence.Height,
            TargetAddress = AddressUtility.ToString(evidence.TargetAddress),
            Timestamp = evidence.Timestamp,
        };
    }
}
