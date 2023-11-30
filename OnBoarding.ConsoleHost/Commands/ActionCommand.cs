using System.ComponentModel.Composition;
using System.Text;
using Bencodex.Types;
using JSSoft.Library.Commands;
using Libplanet.Action.State;
using Libplanet.Blockchain;
using OnBoarding.ConsoleHost.Actions;
using OnBoarding.ConsoleHost.Extensions;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
sealed class ActionCommand(Application application, ActionCollection actions) : CommandMethodBase
{
    private readonly ActionCollection _actions = actions;
    private readonly UserCollection _users = application.GetService<UserCollection>()!;
    private readonly BlockChain _blockChain = application.GetService<BlockChain>()!;

    [CommandProperty]
    public int UserIndex { get; set; }

    [CommandMethod]
    [CommandMethodProperty(nameof(UserIndex))]
    public void Add(int value)
    {
        var userIndex = UserIndex;
        var user = _users[userIndex];
        var action = new AddAction()
        {
            Address = user.Address,
            Value = value
        };
        _actions.Add(action);

        // 블록이 생성하는지 확인하기 위해서 액션 추가후 바로 블록 생성
        var block = BlockChainUtils.AppendNew(_blockChain, user, _users, _actions);
        var sb = new StringBuilder();
        sb.AppendStatesLine(_blockChain, block.Index, _users);
        Out.Write(sb.ToString());
    }
}
