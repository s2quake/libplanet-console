namespace LibplanetConsole.Client;

public readonly record struct ClientInfo
{
    public Address Address { get; init; }

    public Address NodeAddress { get; init; }

    public BlockHash GenesisHash { get; init; }

    public bool IsRunning { get; init; }
}
