#if LIBPLANET_CLIENT || LIBPLANET_CONSOLE
using LibplanetConsole.Client.Grpc;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Client;

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
#endif // LIBPLANET_CLIENT || LIBPLANET_CONSOLE
