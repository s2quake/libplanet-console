using JSSoft.Commands;

namespace LibplanetConsole.Console.Commands.Clients;

[CommandSummary("Starts the client process")]
internal sealed class StartClientProcessCommand(
    ClientProcessCommand processCommand,
    IServiceProvider serviceProvider)
    : ClientCommandAsyncBase(serviceProvider, processCommand, "start")
{
    [CommandPropertySwitch]
    [CommandSummary("If set, the console does not attach to the target process after " +
                    "starting the node process")]
    public bool Detach { get; set; }

    [CommandPropertySwitch]
    [CommandSummary("If set, the client process starts in a new window")]
    public bool NewWindow { get; set; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var client = Client;
        var options = new ProcessOptions
        {
            Detach = Detach,
            NewWindow = NewWindow,
        };
        await client.StartProcessAsync(options, cancellationToken);
    }
}
