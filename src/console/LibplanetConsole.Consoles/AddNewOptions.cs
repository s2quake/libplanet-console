using Libplanet.Crypto;

namespace LibplanetConsole.Consoles;

public sealed record class AddNewOptions
{
    public required PrivateKey PrivateKey { get; init; }

    public bool ManualStart { get; init; }

    public bool NewTerminal { get; init; }

    public bool Detached { get; init; }
}
