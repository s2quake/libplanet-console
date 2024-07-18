using LibplanetConsole.Common;

namespace LibplanetConsole.Explorer;

public sealed record class ExplorerOptions
{
    public required AppEndPoint EndPoint { get; init; }
}
