using LibplanetConsole.Common;

namespace LibplanetConsole.Databases.Serializations;

public readonly record struct DatabaseInfo
{
    public AppEndPoint? EndPoint { get; init; }

    public bool IsRunning { get; init; }
}
