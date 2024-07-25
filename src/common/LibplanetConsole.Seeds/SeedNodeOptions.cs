using LibplanetConsole.Common;

namespace LibplanetConsole.Seeds;

public readonly record struct SeedNodeOptions
{
    public AppEndPoint EndPoint { get; init; }

    public AppPrivateKey PrivateKey { get; init; }
}
