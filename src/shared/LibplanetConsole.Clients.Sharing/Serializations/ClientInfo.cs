using Libplanet.Crypto;

namespace LibplanetConsole.Clients.Serializations;

public sealed record class ClientInfo
{
    public ClientInfo()
    {
    }

    public Address Address { get; init; }

    public Address NodeAddress { get; init; }
}
