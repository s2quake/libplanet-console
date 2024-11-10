namespace LibplanetConsole.Console;

public sealed record class AddNewClientOptions
{
    public required ClientOptions ClientOptions { get; init; }

    public ProcessOptions? ProcessOptions { get; init; }
}
