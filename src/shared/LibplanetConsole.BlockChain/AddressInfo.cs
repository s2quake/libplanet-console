using System.Text.Json.Serialization;
using LibplanetConsole.Grpc.BlockChain;
using static LibplanetConsole.Grpc.TypeUtility;

#if LIBPLANET_NODE
namespace LibplanetConsole.Node;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client;
#elif LIBPLANET_CONSOLE
namespace LibplanetConsole.Console;
#else
#error LIBPLANET_NODE, LIBPLANET_CLIENT, or LIBPLANET_CONSOLE must be defined.
#endif

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
