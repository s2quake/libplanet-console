namespace LibplanetConsole.Client;

public interface IApplicationOptions
{
    PrivateKey PrivateKey { get; }

    int ParentProcessId { get; }

    EndPoint? NodeEndPoint { get; }

    string LogPath { get; }

    bool NoREPL { get; }

    string Alias { get; }
}
