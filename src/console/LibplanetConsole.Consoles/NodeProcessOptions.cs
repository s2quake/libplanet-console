using System.Net;
using System.Security;
using Libplanet.Crypto;
using LibplanetConsole.Common;

namespace LibplanetConsole.Consoles;

internal sealed record class NodeProcessOptions
{
    public NodeProcessOptions(EndPoint endPoint, PrivateKey privateKey)
    {
        EndPoint = endPoint;
        PrivateKey = PrivateKeyUtility.ToSecureString(privateKey);
    }

    public EndPoint EndPoint { get; }

    public SecureString PrivateKey { get; }

    public EndPoint? NodeEndPoint { get; init; }

    public string StoreDirectory { get; init; } = string.Empty;

    public string LogDirectory { get; init; } = string.Empty;

    public bool ManualStart { get; init; }

    public bool NoREPL { get; init; }
}
