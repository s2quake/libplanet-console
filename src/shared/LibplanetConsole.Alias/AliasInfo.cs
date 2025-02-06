using System.Text.Json.Serialization;
using LibplanetConsole.Alias.Grpc;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Alias;

public readonly partial record struct AliasInfo
{
    public AliasInfo()
    {
    }

    public required string Alias { get; init; }

    public required Address Address { get; init; }

    public string[] Tags { get; init; } = [];

    [JsonIgnore]
    public bool IsCustom => Tags.Contains("custom");

    public static implicit operator AliasInfo(AliasInfoProto addressInfo) => new()
    {
        Alias = addressInfo.Alias,
        Address = ToAddress(addressInfo.Address),
        Tags = [.. addressInfo.Tags],
    };

    public static implicit operator AliasInfoProto(AliasInfo addressInfo) => new()
    {
        Alias = addressInfo.Alias,
        Address = ToGrpc(addressInfo.Address),
        Tags = { addressInfo.Tags },
    };
}
