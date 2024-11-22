using JSSoft.Commands;
using LibplanetConsole.Console.Extensions;

namespace LibplanetConsole.Console.Commands.Clients;

[CommandSummary("Start client process")]
internal sealed class StartClientProcessCommand(
    ClientProcessCommand processCommand,
    IClientCollection clients)
    : CommandAsyncBase(processCommand, "start")
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandPropertyCompletion(nameof(GetClientAddresses))]
    [CommandSummary("The address of the client. If not specified, the current client is used")]
    public string Address { get; set; } = string.Empty;

    [CommandPropertySwitch]
    [CommandSummary("The console does not attach to the target process after the client process " +
                    "is started.")]
    public bool Detach { get; set; }

    [CommandPropertySwitch]
    [CommandSummary("The client process is started in a new window")]
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
