using System.Net;
using Libplanet.Crypto;

namespace LibplanetConsole.Consoles;

internal sealed record class ClientProcessOptions
{
    public ClientProcessOptions(EndPoint endPoint, PrivateKey privateKey)
    {
        EndPoint = endPoint;
        PrivateKey = privateKey;
    }

    public EndPoint EndPoint { get; }

    public PrivateKey PrivateKey { get; }

    public EndPoint? NodeEndPoint { get; init; }

    public string LogDirectory { get; init; } = string.Empty;

    public bool ManualStart { get; init; }

    public bool NoREPL { get; init; }
}
