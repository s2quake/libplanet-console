using Libplanet.Crypto;
using LibplanetConsole.Common;

namespace LibplanetConsole.Consoles;

public sealed record class ApplicationOptions
{
    public ApplicationOptions(AppEndPoint endPoint) => EndPoint = endPoint;

    public AppEndPoint EndPoint { get; }

    public PrivateKey[] Nodes { get; init; } = [];

    public PrivateKey[] Clients { get; init; } = [];

    public string StoreDirectory { get; init; } = string.Empty;

    public string LogDirectory { get; init; } = string.Empty;

    public bool NoProcess { get; init; }

    public bool Detach { get; init; }

    public bool NewWindow { get; init; }

    public bool ManualStart { get; init; }
}
