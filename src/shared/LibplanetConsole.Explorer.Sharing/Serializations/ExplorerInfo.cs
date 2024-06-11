namespace LibplanetConsole.Explorer.Serializations;

public readonly record struct ExplorerInfo
{
    public required string EndPoint { get; init; }

    public bool IsRunning { get; init; }
}
