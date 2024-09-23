namespace LibplanetConsole.Consoles;

public sealed record class AddNewNodeOptions
{
    public required NodeOptions NodeOptions { get; init; }

    public bool NoProcess { get; init; }

    public bool NewWindow { get; init; }

    public bool Detach { get; init; }
}
