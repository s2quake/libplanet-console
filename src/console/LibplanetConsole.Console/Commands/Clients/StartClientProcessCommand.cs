using JSSoft.Commands;
using LibplanetConsole.Console.Extensions;

namespace LibplanetConsole.Console.Commands.Clients;

[CommandSummary("Starts the client process")]
internal sealed class StartClientProcessCommand(
    ClientProcessCommand processCommand,
    IClientCollection clients)
    : CommandAsyncBase(processCommand, "start")
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandPropertyCompletion(nameof(GetClientAddresses))]
    [CommandSummary("Specifies the address of the client")]
    public string Address { get; set; } = string.Empty;

    [CommandPropertySwitch]
    [CommandSummary("If set, the console does not attach to the target process after " +
                    "starting the node process")]
    public bool Detach { get; set; }

    [CommandPropertySwitch]
    [CommandSummary("If set, the client process starts in a new window")]
    public bool NewWindow { get; set; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var address = Address;
        var client = clients.GetClientOrCurrent(address);
        var options = new ProcessOptions
        {
            Detach = Detach,
            NewWindow = NewWindow,
        };
        await client.StartProcessAsync(options, cancellationToken);
    }

    private string[] GetClientAddresses()
        => clients.Select(client => client.Address.ToString()).ToArray();
}
