using LibplanetConsole.Grpc.Client;
using static LibplanetConsole.Grpc.TypeUtility;

#if LIBPLANET_CLIENT
namespace LibplanetConsole.Client;
#elif LIBPLANET_CONSOLE
namespace LibplanetConsole.Console;
#else
#error LIBPLANET_CLIENT, or LIBPLANET_CONSOLE must be defined.
#endif

public readonly record struct ClientInfo
{
    public Address Address { get; init; }

    public BlockHash GenesisHash { get; init; }

    public bool IsRunning { get; init; }

    public static ClientInfo Empty { get; } = new ClientInfo { };

    public static implicit operator ClientInfo(ClientInfoProto clientInfo) => new()
    {
        Address = ToAddress(clientInfo.Address),
        GenesisHash = ToBlockHash(clientInfo.GenesisHash),
        IsRunning = clientInfo.IsRunning,
    };

    public static implicit operator ClientInfoProto(ClientInfo clientInfo) => new()
    {
        Address = ToGrpc(clientInfo.Address),
        GenesisHash = ToGrpc(clientInfo.GenesisHash),
        IsRunning = clientInfo.IsRunning,
    };
}
