using LibplanetConsole.Blockchain;
using LibplanetConsole.Client.Grpc;

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

    public static implicit operator ClientInfo(ClientInformation clientInfo)
    {
        return new ClientInfo
        {
            Address = new Address(clientInfo.Address),
            NodeAddress = new Address(clientInfo.NodeAddress),
            GenesisHash = BlockHash.FromString(clientInfo.GenesisHash),
            Tip = new BlockInfo
            {
                Hash = BlockHash.FromString(clientInfo.TipHash),
                Height = clientInfo.TipHeight,
                Miner = new Address(clientInfo.TipMiner),
            },
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
            TipHash = clientInfo.Tip.Hash.ToString(),
            TipHeight = clientInfo.Tip.Height,
            TipMiner = clientInfo.Tip.Miner.ToHex(),
            IsRunning = clientInfo.IsRunning,
        };
    }
}
