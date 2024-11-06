using Libplanet.Net;

namespace LibplanetConsole.Console;

public interface IApplicationOptions
{
    NodeOptions[] Nodes { get; }

    ClientOptions[] Clients { get; }

    byte[] Genesis { get; }

    AppProtocolVersion AppProtocolVersion { get; }

    string LogPath { get; }

    bool NoProcess { get; }

    bool Detach { get; }

    bool NewWindow { get; }

    string ActionProviderModulePath { get; }

    string ActionProviderType { get; }
}
