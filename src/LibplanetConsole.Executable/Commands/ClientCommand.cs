using System.ComponentModel.Composition;
using System.Text;
using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Executable.Extensions;

namespace LibplanetConsole.Executable.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides client-related commands.")]
[method: ImportingConstructor]
sealed class ClientCommand(Application application) : CommandMethodBase
{
    private readonly ClientCollection _clients = application.GetService<ClientCollection>()!;

    [CommandProperty(InitValue = 10)]
    public int Tick { get; set; }

    [CommandMethod]
    public void List()
    {
        var tsb = new TerminalStringBuilder();
        for (var i = 0; i < _clients.Count; i++)
        {
            var item = _clients[i];
            var isCurrent = _clients.Current == item;
            var s = isCurrent == true ? "*" : " ";
            tsb.Append($"{s} ");
            tsb.Foreground = item.IsOnline == true ? (isCurrent == true ? TerminalColorType.BrightGreen : null) : TerminalColorType.BrightBlack;
            tsb.AppendLine($"[{i}]-{item.Address}");
            tsb.ResetOptions();
        }
        Out.Write(tsb.ToString());
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.ClientIndex))]
    public void Login()
    {
        var client = application.GetClient(IndexProperties.ClientIndex);
        var node = application.GetNode(-1);
        client.Login(node);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.ClientIndex))]
    public void Logout()
    {
        var client = application.GetClient(IndexProperties.ClientIndex);
        client.Logout();
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.NodeIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.ClientIndex))]
    public void Status()
    {
        var client = application.GetClient(IndexProperties.ClientIndex);
        var playerInfo = client.GetPlayerInfo();
        Out.WriteLineAsJson(playerInfo);
    }

    [CommandMethod]
    public void Current(int? value = null)
    {
        if (value is { } index)
        {
            _clients.Current = _clients[index];
        }
        else
        {
            Out.WriteLine(_clients.IndexOf(_clients.Current));
        }
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.NodeIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.ClientIndex))]
    public async Task CharacterCreateAsync(CancellationToken cancellationToken)
    {
        var client = application.GetClient(IndexProperties.ClientIndex);
        var node = application.GetNode(IndexProperties.NodeIndex);
        await client.CreateCharacterAsync(node, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.NodeIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.ClientIndex))]
    public async Task CharacterReviveAsync(CancellationToken cancellationToken)
    {
        var client = application.GetClient(IndexProperties.ClientIndex);
        var node = application.GetNode(IndexProperties.NodeIndex);
        await client.ReviveCharacterAsync(node, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.NodeIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.ClientIndex))]
    public void GameHistory()
    {
        var client = application.GetClient(IndexProperties.ClientIndex);
        var node = application.GetNode(IndexProperties.NodeIndex);
        var gamePlayRecords = client.GetGameHistory(node);
        var sb = new StringBuilder();
        sb.AppendLines(gamePlayRecords, item => $"Block #{item.Block.Index}");
        Out.Write(sb.ToString());
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.NodeIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.ClientIndex))]
    public async Task GamePlayAsync(CancellationToken cancellationToken)
    {
        var client = application.GetClient(IndexProperties.ClientIndex);
        var node = application.GetNode(IndexProperties.NodeIndex);
        await client.PlayGameAsync(node, cancellationToken);
        await client.ReplayGameAsync(node, tick: 10, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.NodeIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.ClientIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.BlockIndex))]
    [CommandMethodProperty(nameof(Tick))]
    public async Task GameReplayAsync(CancellationToken cancellationToken)
    {
        var tick = Tick;
        var blockIndex = IndexProperties.BlockIndex;
        var client = application.GetClient(IndexProperties.ClientIndex);
        var node = application.GetNode(IndexProperties.NodeIndex);
        await client.ReplayGameAsync(node, blockIndex, tick, cancellationToken);
    }
}
