using LibplanetConsole.Options;

namespace LibplanetConsole.Client;

public sealed class ApplicationOptions : OptionsBase<ApplicationOptions>
{
    public required int Port { get; init; }

    public required PrivateKey PrivateKey { get; init; }

    public int ParentProcessId { get; init; }

    public EndPoint? NodeEndPoint { get; init; }

    public string LogPath { get; init; } = string.Empty;

    public bool NoREPL { get; init; }
}
