using System.Net;
using Libplanet.Crypto;

namespace LibplanetConsole.Consoles;

public sealed record class ApplicationOptions
{
    public ApplicationOptions(EndPoint endPoint)
    {
        EndPoint = endPoint;
    }

    public EndPoint EndPoint { get; }

    public PrivateKey[] Nodes { get; init; } = [];

    public PrivateKey[] Clients { get; init; } = [];

    public string StoreDirectory { get; init; } = string.Empty;
}
