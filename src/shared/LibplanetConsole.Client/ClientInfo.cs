using GrpcClientInfo = LibplanetConsole.Client.Grpc.ClientInfo;

namespace LibplanetConsole.Client;

public readonly record struct ClientInfo
{
    public Address Address { get; init; }

    public Address NodeAddress { get; init; }

    public BlockHash GenesisHash { get; init; }

    public bool IsRunning { get; init; }

    public static implicit operator GrpcClientInfo(ClientInfo clientInfo)
    {
        return new GrpcClientInfo
        {
            Address = clientInfo.Address.ToHex(),
            NodeAddress = clientInfo.NodeAddress.ToHex(),
            GenesisHash = clientInfo.GenesisHash.ToString(),
            IsRunning = clientInfo.IsRunning,
        };
    }

    public static implicit operator ClientInfo(GrpcClientInfo clientInfo)
    {
        return new ClientInfo
        {
            Address = new Address(clientInfo.Address),
            NodeAddress = new Address(clientInfo.NodeAddress),
            GenesisHash = BlockHash.FromString(clientInfo.GenesisHash),
            IsRunning = clientInfo.IsRunning,
        };
    }
}
