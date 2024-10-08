using LibplanetConsole.Client.Grpc;

namespace LibplanetConsole.Client;

public readonly record struct ClientInfo
{
    public Address Address { get; init; }

    public Address NodeAddress { get; init; }

    public BlockHash GenesisHash { get; init; }

    public BlockHash TipHash { get; init; }

    public bool IsRunning { get; init; }

    public static ClientInfo Empty { get; } = new ClientInfo
    {
    };

    public static implicit operator ClientInfo(ClientInformation clientInfo)
    {
        return new ClientInfo
        {
            Address = new Address(clientInfo.Address),
            NodeAddress = new Address(clientInfo.NodeAddress),
            GenesisHash = BlockHash.FromString(clientInfo.GenesisHash),
            TipHash = BlockHash.FromString(clientInfo.TipHash),
            IsRunning = clientInfo.IsRunning,
        };
    }

    public static implicit operator ClientInformation(ClientInfo clientInfo)
    {
        return new ClientInformation
        {
            Address = clientInfo.Address.ToHex(),
            NodeAddress = clientInfo.NodeAddress.ToHex(),
            GenesisHash = clientInfo.GenesisHash.ToString(),
            TipHash = clientInfo.TipHash.ToString(),
            IsRunning = clientInfo.IsRunning,
        };
    }
}
