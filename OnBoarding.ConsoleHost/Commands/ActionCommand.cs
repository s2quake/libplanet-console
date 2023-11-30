using System.ComponentModel.Composition;
using System.Text;
using Bencodex.Types;
using JSSoft.Library.Commands;
using Libplanet.Action.State;
using Libplanet.Blockchain;
using OnBoarding.ConsoleHost.Actions;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
sealed class ActionCommand(Application application, ActionCollection actions) : CommandMethodBase
{
    private readonly Application _application = application;
    private readonly ActionCollection _actions = actions;
    private readonly UserCollection _users = application.GetService<UserCollection>()!;

    [CommandProperty]
    public int UserIndex { get; set; }

    [CommandMethod]
    [CommandMethodProperty(nameof(UserIndex))]
    public void Add(int value)
    {
        var blockChain = _application.GetService<BlockChain>()!;
        var userIndex = UserIndex;
        var user = _users[userIndex];
        var action = new AddAction()
        {
            Address = user.Address,
            Value = value
        };
        _actions.Add(action);
        var blockIndex = blockChain.Count;
        var block = BlockChainUtils.AppendNew(blockChain, user, _users, [action]);
        var worldState = blockChain.GetWorldState(block.Hash);
        var account = worldState.GetAccount(ReservedAddresses.LegacyAccount);

        var sb = new StringBuilder();
        sb.AppendLine($"Block index #{blockIndex}");
        sb.AppendLine();
        sb.AppendLine("States");
        sb.AppendLine("------");
        foreach (var item in _users)
        {
            var state = account.GetState(item.Address) is Integer i ? (int)i : 0;
            sb.AppendLine($"{item}: {state}");
        }

        Out.WriteLine(sb.ToString());

    }
}
