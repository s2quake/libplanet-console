namespace LibplanetConsole.Client;

public interface IApplicationOptions
{
    PrivateKey PrivateKey { get; }

    int ParentProcessId { get; }

    Uri? HubUrl { get; }

    string LogPath { get; }

    bool NoREPL { get; }
}
