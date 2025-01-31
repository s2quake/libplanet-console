using System.Text.Json.Serialization;
using LibplanetConsole.BlockChain.Grpc;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.BlockChain;

public readonly partial record struct AddressInfo
{
    public AddressInfo()
    {
    }

    public required string Alias { get; init; }

    public required Address Address { get; init; }

    public string[] Tags { get; init; } = [];

    [JsonIgnore]
    public bool IsCustom => Tags.Contains("custom");

    public static implicit operator AddressInfo(AddressInfoProto addressInfo) => new()
    {
        Alias = addressInfo.Alias,
        Address = ToAddress(addressInfo.Address),
        Tags = [.. addressInfo.Tags],
    };

    public static implicit operator AddressInfoProto(AddressInfo addressInfo) => new()
    {
        Alias = addressInfo.Alias,
        Address = ToGrpc(addressInfo.Address),
        Tags = { addressInfo.Tags },
    };
}
