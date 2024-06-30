using Libplanet.Net;
using LibplanetConsole.Common;

namespace LibplanetConsole.Seeds;

public readonly record struct SeedNodeOptions
{
    public AppEndPoint EndPoint { get; init; }

    public AppPrivateKey PrivateKey { get; init; }

    public AppProtocolVersion AppProtocolVersion { get; init; }
}
