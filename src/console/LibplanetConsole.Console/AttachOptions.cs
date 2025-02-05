namespace LibplanetConsole.Console;

public sealed record class AttachOptions
{
    public required Address Address { get; init; }

    public required int ProcessId { get; init; }

    public required Uri Url { get; init; }
}
