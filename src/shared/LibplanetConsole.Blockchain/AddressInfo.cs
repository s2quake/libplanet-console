using LibplanetConsole.Grpc.Blockchain;
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
    public string Alias { get; init; }

    public Address Address { get; init; }

    public static implicit operator AddressInfo(AddressInfoProto addressInfo) => new()
    {
        Alias = addressInfo.Alias,
        Address = ToAddress(addressInfo.Address),
    };

    public static implicit operator AddressInfoProto(AddressInfo addressInfo) => new()
    {
        Alias = addressInfo.Alias,
        Address = ToGrpc(addressInfo.Address),
    };
}
