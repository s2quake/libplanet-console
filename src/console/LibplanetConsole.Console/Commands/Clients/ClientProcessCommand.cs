using JSSoft.Commands;

namespace LibplanetConsole.Console.Commands.Clients;

[CommandSummary("Provides commands for the client process")]
internal sealed class ClientProcessCommand(ClientCommand clientCommand)
    : CommandMethodBase(clientCommand, "process")
{
}
