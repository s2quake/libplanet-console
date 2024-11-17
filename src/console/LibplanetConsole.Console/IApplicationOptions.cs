using Libplanet.Net;

namespace LibplanetConsole.Console;

public interface IApplicationOptions
{
    NodeOptions[] Nodes { get; }

    ClientOptions[] Clients { get; }

    Block GenesisBlock { get; }

    AppProtocolVersion AppProtocolVersion { get; }

    string LogPath { get; }

    ProcessOptions? ProcessOptions { get; }

    string ActionProviderModulePath { get; }

    string ActionProviderType { get; }

    int BlocksyncPort { get; }

    int ConsensusPort { get; }
}
