namespace LibplanetConsole.Consoles;

public sealed record class AddNewClientOptions
{
    public required ClientOptions ClientOptions { get; init; }

    public bool NoProcess { get; init; }

    public bool NewWindow { get; init; }

    public bool Detach { get; init; }
}
