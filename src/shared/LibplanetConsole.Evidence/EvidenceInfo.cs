using Libplanet.Types.Evidence;
using GrpcEvidenceInfo = LibplanetConsole.Evidence.Grpc.EvidenceInfo;

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

    public static implicit operator EvidenceInfo(GrpcEvidenceInfo evidenceInfo)
    {
        return new EvidenceInfo
        {
            Type = evidenceInfo.Type,
            Id = evidenceInfo.Id,
            TargetAddress = new Address(evidenceInfo.TargetAddress),
            Height = evidenceInfo.Height,
            Timestamp = evidenceInfo.Timestamp.ToDateTimeOffset(),
        };
    }

    public static implicit operator GrpcEvidenceInfo(EvidenceInfo evidenceInfo)
    {
        return new GrpcEvidenceInfo
        {
            Type = evidenceInfo.Type,
            Id = evidenceInfo.Id,
            TargetAddress = evidenceInfo.TargetAddress.ToHex(),
            Height = evidenceInfo.Height,
            Timestamp = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(
                evidenceInfo.Timestamp),
        };
    }
}
