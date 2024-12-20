using JSSoft.Commands;

namespace LibplanetConsole.Console.Commands.Clients;

[CommandSummary("Stops the client process")]
internal sealed class StopClientProcessCommand(
    ClientProcessCommand processCommand,
    IServiceProvider serviceProvider)
    : ClientCommandAsyncBase(serviceProvider, processCommand, "stop")
{
    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var client = Client;
        await client.StopProcessAsync(cancellationToken);
    }
}
