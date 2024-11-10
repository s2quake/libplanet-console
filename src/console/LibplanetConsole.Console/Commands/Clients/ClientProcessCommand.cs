using JSSoft.Commands;

namespace LibplanetConsole.Console.Commands.Clients;

[CommandSummary("Provides commands for client processes.")]
internal sealed class ClientProcessCommand(ClientCommand clientCommand)
    : CommandMethodBase(clientCommand, "process")
{
}
