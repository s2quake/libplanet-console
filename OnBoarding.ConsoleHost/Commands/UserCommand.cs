using System.ComponentModel.Composition;
using System.Text;
using JSSoft.Library.Commands;
using JSSoft.Library.Terminals;
using OnBoarding.ConsoleHost.Extensions;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides commands related to users.")]
sealed class UserCommand : CommandMethodBase
{
    private readonly Application _application;
    private readonly UserCollection _users;

    [ImportingConstructor]
    public UserCommand(Application application)
    {
        _application = application;
        _users = application.GetService<UserCollection>()!;
    }

    [CommandProperty(InitValue = 10)]
    public int Tick { get; set; }

    [CommandMethod]
    public void List()
    {
        var tsb = new TerminalStringBuilder();
        for (var i = 0; i < _users.Count; i++)
        {
            var item = _users[i];
            var isCurrent = _users.Current == item;
            var s = isCurrent == true ? "*" : " ";
            tsb.Append($"{s} ");
            tsb.Foreground = item.IsOnline == true ? (isCurrent == true ? TerminalColorType.BrightGreen : null) : TerminalColorType.BrightBlack;
            tsb.AppendLine($"[{i}]-{item.Address}");
            tsb.Foreground = null;
            tsb.Append(string.Empty);
        }
        Out.Write(tsb.ToString());
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.UserIndex))]
    public void Login()
    {
        var user = _application.GetUser(IndexProperties.UserIndex);
        var swarmHost = _application.GetSwarmHost(-1);
        user.Login(swarmHost);
    }

    [CommandMethod]
    public void Logout()
    {
        var user = _application.GetUser(IndexProperties.UserIndex);
        user.Logout();
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.UserIndex))]
    public void Status()
    {
        var user = _application.GetUser(IndexProperties.UserIndex);
        if (user.IsOnline == false)
            throw new InvalidOperationException($"User '{user.Address}' is not online.");
        if (user.PlayerInfo == null)
            throw new InvalidOperationException($"User '{user.Address}' does not have character.");

        Out.WriteLineAsJson(user.PlayerInfo);
    }

    [CommandMethod]
    public void Current(int? value = null)
    {
        if (value is { } index)
        {
            _users.Current = _users[index];
        }
        else
        {
            Out.WriteLine(_users.IndexOf(_users.Current));
        }
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.UserIndex))]
    public async Task CharacterCreateAsync(CancellationToken cancellationToken)
    {
        var user = _application.GetUser(IndexProperties.UserIndex);
        if (user.IsOnline == false)
            throw new InvalidOperationException($"'{Name}' is not online.");
        if (user.PlayerInfo != null)
            throw new InvalidOperationException($"The character of user '{user.Address}' has already been created.");

        var swarmHost = _application.GetSwarmHost(IndexProperties.SwarmIndex);
        await user.CreateCharacterAsync(swarmHost, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.UserIndex))]
    public void GameHistory()
    {
        var blockChain = _application.GetBlockChain(IndexProperties.SwarmIndex);
        var user = _application.GetUser(IndexProperties.UserIndex);
        if (user.IsOnline == false)
            throw new InvalidOperationException($"'{Name}' is not online.");

        var stageRecords = GamePlayRecord.GetGamePlayRecords(blockChain, user.Address);
        var sb = new StringBuilder();
        foreach (var item in stageRecords)
        {
            sb.AppendLine($"Block #{item.Block.Index}");
        }
        Out.Write(sb.ToString());
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.UserIndex))]
    public async Task GamePlayAsync(CancellationToken cancellationToken)
    {
        var user = _application.GetUser(IndexProperties.UserIndex);
        if (user.IsOnline == false)
            throw new InvalidOperationException($"'{Name}' is not online.");
        if (user.PlayerInfo == null)
            throw new InvalidOperationException($"User '{user.Address}' does not have character.");

        var swarmHost = _application.GetSwarmHost(IndexProperties.SwarmIndex);
        await user.PlayGameAsync(swarmHost, cancellationToken);
        await user.ReplayGameAsync(swarmHost, tick: 10, Out, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.UserIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.BlockIndex))]
    [CommandMethodProperty(nameof(Tick))]
    public async Task GameReplayAsync(CancellationToken cancellationToken)
    {
        var tick = Tick;
        var blockIndex = IndexProperties.BlockIndex;
        var user = _application.GetUser(IndexProperties.UserIndex);
        var swarmHost = _application.GetSwarmHost(IndexProperties.SwarmIndex);
        await user.ReplayGameAsync(swarmHost, blockIndex, tick, Out, cancellationToken);
    }
}
