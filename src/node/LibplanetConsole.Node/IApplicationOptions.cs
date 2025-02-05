namespace LibplanetConsole.Node;

public interface IApplicationOptions
{
    PrivateKey PrivateKey { get; }

    Block GenesisBlock { get; }

    AppProtocolVersion AppProtocolVersion { get; }

    int ParentProcessId { get; }

    Uri? HubUrl { get; }

    string StorePath { get; }

    string LogPath { get; }

    bool NoREPL { get; }

    bool IsSingleNode { get; }

    string ActionProviderModulePath { get; }

    string ActionProviderType { get; }

    int BlocksyncPort { get; }

    int ConsensusPort { get; }
}
