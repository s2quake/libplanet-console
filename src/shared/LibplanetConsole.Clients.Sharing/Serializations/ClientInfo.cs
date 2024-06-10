using Libplanet.Crypto;
using Libplanet.Types.Blocks;

namespace LibplanetConsole.Clients.Serializations;

public readonly record struct ClientInfo
{
    public Address Address { get; init; }

    public Address NodeAddress { get; init; }

    public BlockHash GenesisHash { get; init; }

    public bool IsRunning { get; init; }

    public static ClientInfo Empty { get; } = new ClientInfo
    {
    };
}
