namespace LibplanetConsole.Console;

public interface IApplicationOptions
{
    PrivateKey PrivateKey { get; }

    NodeOptions[] Nodes { get; }

    ClientOptions[] Clients { get; }

    Block GenesisBlock { get; }

    string AppProtocolVersion { get; }

    string LogPath { get; }

    ProcessOptions? ProcessOptions { get; }

    string ActionProviderModulePath { get; }

    string ActionProviderType { get; }

    int BlocksyncPort { get; }

    int ConsensusPort { get; }

    bool NoProcess { get; }

    bool Detach { get; }

    bool NewWindow { get; }
}
