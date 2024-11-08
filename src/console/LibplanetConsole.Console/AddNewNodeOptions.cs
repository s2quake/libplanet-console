namespace LibplanetConsole.Console;

public sealed record class AddNewNodeOptions
{
    public required NodeOptions NodeOptions { get; init; }

    public ProcessOptions? ProcessOptions { get; init; }
}
