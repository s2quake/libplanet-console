using Libplanet.Crypto;
using Libplanet.Types.Blocks;

namespace LibplanetConsole.Clients.Serializations;

public sealed record class ClientInfo
{
    public Address Address { get; init; }

    public Address NodeAddress { get; init; }

    public BlockHash GenesisHash { get; init; }
}
