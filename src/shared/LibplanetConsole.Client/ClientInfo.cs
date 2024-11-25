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

    public BlockInfo Tip { get; init; }

    public bool IsRunning { get; init; }

    public static ClientInfo Empty { get; } = new ClientInfo
    {
        Tip = BlockInfo.Empty,
    };

    public static implicit operator ClientInfo(ClientInfoProto clientInfo)
    {
        return new ClientInfo
        {
            Address = ToAddress(clientInfo.Address),
            Tip = clientInfo.Tip,
            IsRunning = clientInfo.IsRunning,
        };
    }

    public static implicit operator ClientInfoProto(ClientInfo clientInfo)
    {
        return new ClientInfoProto
        {
            Address = ToGrpc(clientInfo.Address),
            Tip = clientInfo.Tip,
            IsRunning = clientInfo.IsRunning,
        };
    }
}
