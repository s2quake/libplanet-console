namespace LibplanetConsole.Console;

public interface IApplicationOptions
{
    int Port { get; }

    NodeOptions[] Nodes { get; }

    ClientOptions[] Clients { get; }

    byte[] Genesis { get; }

    string LogPath { get; }

    bool NoProcess { get; }

    bool Detach { get; }

    bool NewWindow { get; }
}
