using System.ComponentModel.Composition;
using JSSoft.Commands;
using JSSoft.Terminals;

namespace LibplanetConsole.Executable.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
sealed class BotCommand(Application application) : CommandMethodBase
{
    private readonly ClientCollection _clients = application.GetService<ClientCollection>()!;
    private readonly BotCollection _bots = application.GetService<BotCollection>()!;

    [CommandMethod]
    public async Task StartAllAsync(CancellationToken cancellationToken)
    {
        for (var i = 0; i < _clients.Count; i++)
        {
            var client = _clients[i];
            var bot = _bots.Contains(client) == true ? _bots[client] : _bots.AddNew(client);
            if (bot.IsRunning == false)
            {
                await bot.StartAsync(cancellationToken);
            }
        }
    }

    [CommandMethod]
    public async Task StopAllAsync(CancellationToken cancellationToken)
    {
        var query = from item in _bots
                    where item.IsRunning == true
                    select item.StopAsync(cancellationToken);
        await Task.WhenAll(query);
    }

    [CommandMethod]
    public async Task StartAsync(int clientIndex, CancellationToken cancellationToken)
    {
        var client = _clients[clientIndex];
        var bot = _bots.Contains(client) == true ? _bots[client] : _bots.AddNew(client);
        await bot.StartAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task StopAsync(int clientIndex, CancellationToken cancellationToken)
    {
        var bot = _bots[clientIndex];
        await bot.StopAsync(cancellationToken);
    }

    [CommandMethod]
    public void List()
    {
        var tsb = new TerminalStringBuilder();
        for (var i = 0; i < _bots.Count; i++)
        {
            var item = _bots[i];
            var isRunning = item.IsRunning;
            tsb.Foreground = isRunning == true ? null : TerminalColorType.BrightBlack;
            tsb.AppendLine($"[{i}]-{item.Client.Address}");
            tsb.Foreground = null;
            tsb.Append(string.Empty);
        }
        Out.Write(tsb.ToString());
    }
}
