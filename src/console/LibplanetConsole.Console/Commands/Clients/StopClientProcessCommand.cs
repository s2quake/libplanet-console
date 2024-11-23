using JSSoft.Commands;
using LibplanetConsole.Console.Extensions;

namespace LibplanetConsole.Console.Commands.Clients;

[CommandSummary("Stops the client process")]
internal sealed class StopClientProcessCommand(
    ClientProcessCommand processCommand,
    IClientCollection clients)
    : CommandAsyncBase(processCommand, "stop")
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandPropertyCompletion(nameof(GetClientAddresses))]
    [CommandSummary("Specifies the address of the client")]
    public string Address { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var address = Address;
        var client = clients.GetClientOrCurrent(address);
        await client.StopProcessAsync(cancellationToken);
    }

    private string[] GetClientAddresses()
        => clients.Select(client => client.Address.ToString()).ToArray();
}
