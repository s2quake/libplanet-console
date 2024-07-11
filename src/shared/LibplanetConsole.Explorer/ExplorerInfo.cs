using LibplanetConsole.Common;

namespace LibplanetConsole.Explorer;

public readonly record struct ExplorerInfo
{
    public AppEndPoint? EndPoint { get; init; }

    public bool IsRunning { get; init; }
}
