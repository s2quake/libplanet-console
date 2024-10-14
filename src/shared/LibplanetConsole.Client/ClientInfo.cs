using LibplanetConsole.Blockchain;
using LibplanetConsole.Client.Grpc;
using static LibplanetConsole.Blockchain.Grpc.TypeUtility;

namespace LibplanetConsole.Client;

public readonly record struct ClientInfo
{
    public Address Address { get; init; }

    public Address NodeAddress { get; init; }

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
            NodeAddress = ToAddress(clientInfo.NodeAddress),
            GenesisHash = ToBlockHash(clientInfo.GenesisHash),
            Tip = clientInfo.Tip,
            IsRunning = clientInfo.IsRunning,
        };
    }

    public static implicit operator ClientInfoProto(ClientInfo clientInfo)
    {
        return new ClientInfoProto
        {
            Address = ToGrpc(clientInfo.Address),
            NodeAddress = ToGrpc(clientInfo.NodeAddress),
            GenesisHash = ToGrpc(clientInfo.GenesisHash),
            Tip = clientInfo.Tip,
            IsRunning = clientInfo.IsRunning,
        };
    }
}
