namespace LibplanetConsole.Console;

public sealed record class ProcessOptions
{
    public bool NewWindow { get; init; }

    public bool Detach { get; init; }

    public static ProcessOptions Default { get; } = new();
}
