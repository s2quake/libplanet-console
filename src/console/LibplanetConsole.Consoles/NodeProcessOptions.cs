using System.Net;
using Libplanet.Crypto;

namespace LibplanetConsole.Consoles;

internal sealed record class NodeProcessOptions
{
    public NodeProcessOptions(EndPoint endPoint, PrivateKey privateKey)
    {
        EndPoint = endPoint;
        PrivateKey = privateKey;
    }

    public EndPoint EndPoint { get; }

    public PrivateKey PrivateKey { get; }

    public string StoreDirectory { get; init; } = string.Empty;

    public string LogDirectory { get; init; } = string.Empty;
}
