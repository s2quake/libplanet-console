namespace LibplanetConsole.Node;

public interface IApplicationOptions
{
    PrivateKey PrivateKey { get; }

    byte[] Genesis { get; }

    int ParentProcessId { get; }

    EndPoint? SeedEndPoint { get; }

    string StorePath { get; }

    string LogPath { get; }

    bool NoREPL { get; }

    string ActionProviderModulePath { get; }

    string ActionProviderType { get; }
}
