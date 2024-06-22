using LibplanetConsole.Common;

namespace LibplanetConsole.Explorer.Serializations;

public readonly record struct ExplorerInfo
{
    public required AppEndPoint EndPoint { get; init; }

    public bool IsRunning { get; init; }
}
