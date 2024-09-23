using Libplanet.Types.Blocks;
using LibplanetConsole.Common;

namespace LibplanetConsole.Clients;

public readonly record struct ClientInfo
{
    public AppAddress Address { get; init; }

    public AppAddress NodeAddress { get; init; }

    public BlockHash GenesisHash { get; init; }

    public bool IsRunning { get; init; }
}
