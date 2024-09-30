using LibplanetConsole.Common;

namespace LibplanetConsole.Explorer;

public readonly record struct ExplorerInfo
{
    public EndPoint? EndPoint { get; init; }

    public bool IsRunning { get; init; }
}
