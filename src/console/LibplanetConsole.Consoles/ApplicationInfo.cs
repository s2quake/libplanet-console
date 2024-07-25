using LibplanetConsole.Common;

namespace LibplanetConsole.Consoles;

public readonly record struct ApplicationInfo
{
    public required AppEndPoint EndPoint { get; init; }

    public required string StoreDirectory { get; init; }

    public required string LogDirectory { get; init; }

    public bool NoProcess { get; init; }

    public bool Detach { get; init; }

    public bool ManualStart { get; init; }

    public bool NewWindow { get; init; }
}
