namespace LibplanetConsole.Client;

public interface IApplicationOptions
{
    int Port { get; }

    PrivateKey PrivateKey { get; }

    int ParentProcessId { get; }

    EndPoint? NodeEndPoint { get; }

    string LogPath { get; }

    bool NoREPL { get; }
}
