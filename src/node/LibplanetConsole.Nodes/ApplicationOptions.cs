using System.Net;
using Libplanet.Crypto;

namespace LibplanetConsole.Nodes;

public sealed record class ApplicationOptions
{
    public ApplicationOptions(EndPoint endPoint, PrivateKey privateKey)
    {
        EndPoint = endPoint;
        PrivateKey = privateKey;
    }

    public EndPoint EndPoint { get; }

    public PrivateKey PrivateKey { get; }

    public int ParentProcessId { get; init; }

    public bool AutoStart { get; init; } = false;

    public EndPoint? NodeEndPoint { get; init; }

    public string StorePath { get; init; } = string.Empty;

    public string LogPath { get; set; } = string.Empty;

    public PublicKey[] GenesisValidators { get; init; } = [];
}
