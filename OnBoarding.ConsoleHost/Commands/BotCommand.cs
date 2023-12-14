using System.ComponentModel.Composition;
using JSSoft.Library.Commands;
using JSSoft.Library.Terminals;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
sealed class BotCommand : CommandMethodBase
{
    private readonly Application _application;
    private readonly UserCollection _users;
    private readonly BotCollection _bots;

    [ImportingConstructor]
    public BotCommand(Application application)
    {
        _application = application;
        _users = application.GetService<UserCollection>()!;
        _bots = application.GetService<BotCollection>()!;
    }

    [CommandMethod]
    public async Task StartAllAsync(CancellationToken cancellationToken)
    {
        for (var i = 0; i < _users.Count; i++)
        {
            var user = _users[i];
            var bot = _bots.Contains(user) == true ? _bots[user] : _bots.AddNew(user);
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
    public async Task StartAsync(int userIndex, CancellationToken cancellationToken)
    {
        var user = _users[userIndex];
        var bot = _bots.Contains(user) == true ? _bots[user] : _bots.AddNew(user);
        await bot.StartAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task StopAsync(int userIndex, CancellationToken cancellationToken)
    {
        var bot = _bots[userIndex];
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
            tsb.AppendLine($"[{i}]-{item.User.Address}");
            tsb.Foreground = null;
            tsb.Append(string.Empty);
        }
        Out.Write(tsb.ToString());
    }
}
