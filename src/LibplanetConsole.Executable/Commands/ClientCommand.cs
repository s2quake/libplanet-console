using System.ComponentModel.Composition;
using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.ClientServices;

namespace LibplanetConsole.Executable.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides client-related commands.")]
[method: ImportingConstructor]
internal sealed class ClientCommand(IApplication application, ClientCollection clients)
    : CommandMethodBase
{
    [CommandProperty(InitValue = 10)]
    public int Tick { get; set; }

    [CommandMethod]
    public void List()
    {
        var tsb = new TerminalStringBuilder();
        for (var i = 0; i < clients.Count; i++)
        {
            var item = clients[i];
            var isCurrent = clients.Current == item;
            tsb.Foreground = GetForeground(client: item, isCurrent);
            tsb.IsBold = item.IsRunning == true;
            tsb.AppendLine($"{item}");
            tsb.ResetOptions();
        }

        Out.Write(tsb.ToString());
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.Client))]
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var clientOptions = new ClientOptions();
        var client = application.GetClient(IndexProperties.Client);
        await client.StartAsync(clientOptions, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.Client))]
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var client = application.GetClient(IndexProperties.Client);
        await client.StopAsync(cancellationToken);
    }

    [CommandMethod]
    public void Current(string? identifier = null)
    {
        if (identifier is not null && application.GetClient(identifier) is { } client)
        {
            clients.Current = client;
        }

        if (clients.Current is not null)
        {
            Out.WriteLine(clients.Current);
        }
        else
        {
            Out.WriteLine("No client is selected.");
        }
    }

    private static TerminalColorType? GetForeground(IClient client, bool isCurrent)
    {
        if (client.IsRunning == true)
        {
            return isCurrent == true ? TerminalColorType.BrightGreen : null;
        }

        return TerminalColorType.BrightBlack;
    }
}
