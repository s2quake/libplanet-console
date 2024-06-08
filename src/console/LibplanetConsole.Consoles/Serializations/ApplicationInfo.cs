namespace LibplanetConsole.Consoles.Serializations;

public sealed record class ApplicationInfo
{
    public string EndPoint { get; init; } = string.Empty;

    public string StoreDirectory { get; init; } = string.Empty;

    public string LogDirectory { get; init; } = string.Empty;

    public bool NoProcess { get; init; }

    public bool Detach { get; init; }

    public bool ManualStart { get; init; }

    public bool NewWindow { get; init; }
}
