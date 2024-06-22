using LibplanetConsole.Common;

namespace LibplanetConsole.Explorer.Serializations;

public readonly record struct ExplorerInfo
{
    public AppEndPoint? EndPoint { get; init; }

    public bool IsRunning { get; init; }
}
